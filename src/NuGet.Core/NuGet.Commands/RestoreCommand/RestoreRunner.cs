using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Logging;
using NuGet.Protocol.Core.Types;

namespace NuGet.Commands
{
    public class RestoreRunner
    {
        private readonly IReadOnlyList<IRestoreRequestProvider> _providers;
        private readonly ISettings _settings;
        private static readonly int MaxDegreesOfConcurrency = Environment.ProcessorCount;
        private readonly List<SourceRepository> _sources;
        private readonly SourceCacheContext _cacheContext;

        public RestoreRunner(
            List<IRestoreRequestProvider> providers,
            ISettings settings,
            List<SourceRepository> sources,
            SourceCacheContext cacheContext)
        {
            _providers = providers;
            _settings = settings;
            _sources = sources;
            _cacheContext = cacheContext;

            DisableParallel = RuntimeEnvironmentHelper.IsMono;
        }

        public async Task<int> Run(ILogger log, IEnumerable<string> inputs, LogLevel logLevel)
        {
            var maxTasks = DisableParallel ? 1 : MaxDegreesOfConcurrency;

            if (maxTasks < 1)
            {
                maxTasks = 1;
            }

            // TODO: fix strings
            if (!DisableParallel)
            {
                log.LogVerbose(string.Format(
                    CultureInfo.CurrentCulture,
                    "Strings.Log_RunningParallelRestore",
                    maxTasks));
            }
            else
            {
                log.LogVerbose("Strings.Log_RunningNonParallelRestore");
            }

            // Get requests
            var requests = new Queue<RestoreSummaryRequest>();
            var restoreTasks = new List<Task<RestoreSummary>>();
            var restoreSummaries = new List<RestoreSummary>();

            foreach (var input in inputs)
            {
                foreach (var request in await CreateRequests(
                    input,
                    GlobalPackagesFolder,
                    _sources,
                    _cacheContext,
                    log))
                {
                    requests.Enqueue(request);
                }
            }

            // Run requests
            while (requests.Count > 0)
            {
                // Throttle and wait for a task to finish if we have hit the limit
                if (restoreTasks.Count == maxTasks)
                {
                    var restoreSummary = await CompleteTaskAsync(restoreTasks);
                    restoreSummaries.Add(restoreSummary);
                }

                var task = Task.Run(async () => await Execute(requests.Dequeue()));
            }

            // Wait for all restores to finish
            while (restoreTasks.Count > 0)
            {
                var restoreSummary = await CompleteTaskAsync(restoreTasks);
                restoreSummaries.Add(restoreSummary);
            }

            // Summary
            RestoreSummary.Log(log, restoreSummaries, logLevel < LogLevel.Minimal);

            return restoreSummaries.All(x => x.Success) ? 0 : 1;
        }

        private async Task<RestoreSummary> Execute(RestoreSummaryRequest summaryRequest)
        {
            // Run the restore
            var request = summaryRequest.Request;
            var command = new RestoreCommand(request);
            var sw = Stopwatch.StartNew();
            var result = await command.ExecuteAsync();

            // Commit the result
            request.Log.LogInformation("Strings.Log_Committing");
            result.Commit(request.Log);

            sw.Stop();

            if (result.Success)
            {
                request.Log.LogMinimal(string.Format(
                    CultureInfo.CurrentCulture,
                    "Strings.Log_RestoreComplete",
                    sw.ElapsedMilliseconds));
            }
            else
            {
                request.Log.LogMinimal(string.Format(
                    CultureInfo.CurrentCulture,
                    "Strings.Log_RestoreFailed",
                    sw.ElapsedMilliseconds));
            }

            // Build the summary
            return new RestoreSummary(
                result,
                summaryRequest.InputPath,
                summaryRequest.Settings,
                summaryRequest.Sources,
                summaryRequest.CollectorLogger.Errors);
        }

        private static async Task<RestoreSummary> CompleteTaskAsync(List<Task<RestoreSummary>> restoreTasks)
        {
            var doneTask = await Task.WhenAny(restoreTasks);
            restoreTasks.Remove(doneTask);
            return await doneTask;
        }

        /// <summary>
        /// Override parallel settings
        /// </summary>
        public bool DisableParallel { get; set; }

        /// <summary>
        /// Override the global folder location
        /// </summary>
        public string GlobalPackagesFolder { get; set; }

        private async Task<IReadOnlyList<RestoreSummaryRequest>> CreateRequests(
            string input,
            string globalPackagesPath,
            List<SourceRepository> sources,
            SourceCacheContext cacheContext,
            ILogger log)
        {
            foreach (var provider in _providers)
            {
                if (await provider.Supports(input))
                {
                    return await provider.CreateRequests(
                        input,
                        globalPackagesPath,
                        sources,
                        cacheContext,
                        log);
                }
            }

            throw new ArgumentException(input);
        }
    }
}
