﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Dnx.Runtime.Common.CommandLine;
using NuGet.Common;
using NuGet.Logging;
using NuGet.Protocol;

namespace NuGet.CommandLine.XPlat
{
    public class Program
    {
        public static CommandOutputLogger Log { get; set; }

        public static int Main(string[] args)
        {
#if DEBUG
            if (args.Contains("--debug"))
            {
                args = args.Where(arg => arg != "--debug").ToArray();

                while (!Debugger.IsAttached)
                {
                    System.Threading.Thread.Sleep(100);
                }

                Debugger.Break();
            }
#endif

            // First, optionally disable localization in resources.
            var invariantResources = new List<Type>();
            if (args.Any(arg => string.Equals(arg, "--force-invariant", StringComparison.OrdinalIgnoreCase)))
            {
                invariantResources.AddRange(StringResource.DisableLocalizationInNuGetResources());
            }

            var app = new CommandLineApplication();
            app.Name = "nuget3";
            app.FullName = Strings.App_FullName;
            app.HelpOption(XPlatUtility.HelpOption);
            app.VersionOption("--version", typeof(Program).GetTypeInfo().Assembly.GetName().Version.ToString());

            var verbosity = app.Option(XPlatUtility.VerbosityOption, Strings.Switch_Verbosity, CommandOptionType.SingleValue);

            // Set up logging.
            // For tests this will already be set.
            if (Log == null)
            {
                var logLevel = XPlatUtility.GetLogLevel(verbosity);
                Log = new CommandOutputLogger(logLevel);
            }

            XPlatUtility.SetConnectionLimit();

            XPlatUtility.SetUserAgent();

            // This method has no effect on .NET Core.
            NetworkProtocolUtility.ConfigureSupportedSslProtocols();

            // Register commands
            DeleteCommand.Register(app, () => Log);
            PackCommand.Register(app, () => Log);
            PushCommand.Register(app, () => Log);
            RestoreCommand.Register(app, () => Log);

            app.OnExecute(() =>
            {
                app.ShowHelp();

                // Report disabled localization.
                if (invariantResources.Any())
                {
                    Log.LogDebug(Environment.NewLine + "Localization was disabled on the following types:");

                    foreach (var invariantResource in invariantResources)
                    {
                        Log.LogDebug($" - {invariantResource.FullName}");
                    }
                }

                return 0;
            });

            int exitCode = 0;

            try
            {
                exitCode = app.Execute(args);
            }
            catch (Exception e)
            {
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
    }
}
