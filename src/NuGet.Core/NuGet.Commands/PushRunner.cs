﻿using System.Threading.Tasks;
using NuGet.Configuration;
using NuGet.Common;
using NuGet.Protocol.Core.Types;

namespace NuGet.Commands
{
    /// <summary>
    /// Shared code to run the "push" command from the command line projects
    /// </summary>
    public static class PushRunner
    {
        public static async Task Run(
            ISettings settings,
            IPackageSourceProvider sourceProvider,
            string packagePath,
            string source,
            string apiKey,
            int timeoutSeconds,
            bool disableBuffering,
            bool noSymbols,
            ILogger logger)
        {
            source = CommandRunnerUtility.ResolveSource(sourceProvider, source);

            if (timeoutSeconds == 0)
            {
                timeoutSeconds = 5 * 60;
            }

            PackageUpdateResource packageUpdateResource = await CommandRunnerUtility.GetPackageUpdateResource(sourceProvider, source);

            string symbolsSource = !noSymbols
                ? NuGetConstants.DefaultSymbolServerUrl
                : string.Empty;

            await packageUpdateResource.Push(
                packagePath,
                symbolsSource,
                timeoutSeconds,
                disableBuffering,
                endpoint => CommandRunnerUtility.GetApiKey(settings, endpoint, apiKey),
                logger);
        }
    }
}
