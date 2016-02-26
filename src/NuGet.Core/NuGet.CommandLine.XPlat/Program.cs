// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using Microsoft.Dnx.Runtime.Common.CommandLine;
using NuGet.Commands;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Logging;
using NuGet.Protocol.Core.Types;
using NuGet.Protocol.Core.v3;

namespace NuGet.CommandLine.XPlat
{
    public class Program
    {
        private const string HelpOption = "-h|--help";
        private const string VerbosityOption = "-v|--verbosity <verbosity>";
        private static readonly int MaxDegreesOfConcurrency = Environment.ProcessorCount;

        public static ILogger Log { get; set; }

        public static int Main(string[] args)
        {
#if DEBUG
            if (args.Contains("--debug"))
            {
                args = args.Skip(1).ToArray();
                while (!Debugger.IsAttached)
                {

                }
                Debugger.Break();
            }
#endif

            var app = new CommandLineApplication();
            app.Name = "nuget3";
            app.FullName = Strings.App_FullName;
            app.HelpOption(HelpOption);
            app.VersionOption("--version", typeof(Program).GetTypeInfo().Assembly.GetName().Version.ToString());

            var verbosity = app.Option(VerbosityOption, Strings.Switch_Verbosity, CommandOptionType.SingleValue);

            SetConnectionLimit();

            SetUserAgent();

            //register push and delete command
            new PushCommand(app, () => {
                EnsureLog(GetLogLevel(verbosity));
                return Log;
            });
            new DeleteCommand(app, () => {
                EnsureLog(GetLogLevel(verbosity));
                return Log;
            });

            app.Command("restore", (Action<CommandLineApplication>)(restore =>
            {
                restore.Description = Strings.Restore_Description;
                restore.HelpOption(HelpOption);

                var sources = restore.Option(
                    "-s|--source <source>",
                    Strings.Restore_Switch_Source_Description,
                    CommandOptionType.MultipleValue);

                var packagesDirectory = restore.Option(
                    "--packages <packagesDirectory>",
                    Strings.Restore_Switch_Packages_Description,
                    CommandOptionType.SingleValue);

                var disableParallel = restore.Option(
                    "--disable-parallel",
                    Strings.Restore_Switch_DisableParallel_Description,
                    CommandOptionType.NoValue);

                var fallBack = restore.Option(
                    "-f|--fallbacksource <FEED>",
                    Strings.Restore_Switch_Fallback_Description,
                    CommandOptionType.MultipleValue);

                var runtime = restore.Option(
                    "--runtime <RID>",
                    Strings.Restore_Switch_Runtime_Description,
                    CommandOptionType.MultipleValue);

                var configFile = restore.Option(
                    "--configfile <file>",
                    Strings.Restore_Switch_ConfigFile_Description,
                    CommandOptionType.SingleValue);

                verbosity = restore.Option(
                    VerbosityOption,
                    Strings.Switch_Verbosity,
                    CommandOptionType.SingleValue);

                var argRoot = restore.Argument(
                    "[root]",
                    Strings.Restore_Arg_ProjectName_Description,
                    multipleValues: true);

                restore.OnExecute(async () =>
                {
                    var logLevel = GetLogLevel(verbosity);
                    EnsureLog(logLevel);

                    using (var cacheContext = new SourceCacheContext())
                    {
                        var providerCache = new RestoreCommandProvidersCache();

                        // Ordered request providers
                        var providers = new List<IRestoreRequestProvider>();
                        providers.Add(new MSBuildP2PRestoreRequestProvider(providerCache));
                        providers.Add(new ProjectJsonRestoreRequestProvider(providerCache));

                        var restoreContext = new RestoreArgs()
                        {
                            CacheContext = cacheContext,
                            ConfigFileName = configFile.HasValue() ? configFile.Value() : null,
                            DisableParallel = disableParallel.HasValue(),
                            GlobalPackagesFolder = packagesDirectory.HasValue() ? packagesDirectory.Value() : null,
                            Inputs = new List<string>(argRoot.Values),
                            Log = Log,
                            MachineWideSettings = new CommandLineXPlatMachineWideSetting(),
                            RequestProviders = providers,
                            Sources = sources.Values,
                            FallbackSources = fallBack.Values,
                            CachingSourceProvider = _sourceProvider
                        };

                        restoreContext.Runtimes.UnionWith(runtime.Values);

                        var restoreSummaries = await RestoreRunner.Run(restoreContext);

                        // Summary
                        RestoreSummary.Log(Log, restoreSummaries, logLevel < LogLevel.Minimal);

                        return restoreSummaries.All(x => x.Success) ? 0 : 1;
                    }
                });
            }));

            app.OnExecute(() =>
            {
                app.ShowHelp();
                return 0;
            });

            var exitCode = 0;

            try
            {
                exitCode = app.Execute(args);
            }
            catch (Exception e)
            {
                EnsureLog(GetLogLevel(verbosity));

                // Log the error
                Log.LogError(ExceptionUtilities.DisplayMessage(e));

                // Log the stack trace as verbose output.
                Log.LogVerbose(e.ToString());

                exitCode = 1;
            }

            // Limit the exit code range to 0-255 to support POSIX
            if (exitCode < 0 || exitCode > 255)
            {
                exitCode = 1;
            }

            return exitCode;
        }

        private static LogLevel GetLogLevel(CommandOption verbosity)
        {
            LogLevel level;
            if (!Enum.TryParse(value: verbosity.Value(), ignoreCase: true, result: out level))
            {
                level = LogLevel.Information;
            }

            return level;
        }

        private static void SetUserAgent()
        {
            UserAgent.UserAgentString
                = UserAgent.CreateUserAgentString(
                    $"NuGet xplat");
        }

        private static void EnsureLog(LogLevel logLevel)
        {
            // Set up logging.
            // For tests this will already be set.
            if (Log == null)
            {
                Log = new CommandOutputLogger(logLevel);
            }
        }

        private static void SetConnectionLimit()
        {
#if !DNXCORE50
            // Increase the maximum number of connections per server.
            if (!RuntimeEnvironmentHelper.IsMono)
            {
                ServicePointManager.DefaultConnectionLimit = 64;
            }
            else
            {
                // Keep mono limited to a single download to avoid issues.
                ServicePointManager.DefaultConnectionLimit = 1;
            }
#endif
        }

        // Create a caching source provider with the default settings, the sources will be passed in
        private static CachingSourceProvider _sourceProvider = new CachingSourceProvider(
            new PackageSourceProvider(
                Settings.LoadDefaultSettings(root: null, configFileName: null, machineWideSettings: null)));
    }
}
