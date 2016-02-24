using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Logging;
using NuGet.ProjectModel;
using NuGet.Protocol.Core.Types;

namespace NuGet.Commands
{
    public class ProjectJsonRestoreRequestProvider : IRestoreRequestProvider
    {
        private readonly RestoreCommandProvidersCache _providerCache;
        private readonly RequestSettingsProvider _settingsProvider;

        public ProjectJsonRestoreRequestProvider(
            RestoreCommandProvidersCache providerCache,
            RequestSettingsProvider settingsProvider)
        {
            _providerCache = providerCache;
            _settingsProvider = settingsProvider;
        }

        public Task<IReadOnlyList<RestoreSummaryRequest>> CreateRequests(
            string inputPath,
            string globalPackagesPath,
            List<SourceRepository> sources,
            SourceCacheContext cacheContext,
            ILogger log)
        {
            var paths = new List<string>();

            if (Directory.Exists(inputPath))
            {
                paths.AddRange(GetProjectJsonFilesInDirectory(inputPath));
            }
            else
            {
                paths.Add(inputPath);
            }

            var requests = new List<RestoreSummaryRequest>(paths.Count);

            foreach (var path in paths)
            {
                requests.Add(Create(path, globalPackagesPath, sources, cacheContext, log));
            }

            return Task.FromResult<IReadOnlyList<RestoreSummaryRequest>>(requests);
        }

        public Task<bool> Supports(string path)
        {
            // True if dir or project.json file
            var result = Directory.Exists(path) 
                || (File.Exists(path) && ProjectJsonPathUtilities.IsProjectConfig(path));

            return Task.FromResult(result);
        }

        private RestoreSummaryRequest Create(
            string inputPath,
            string globalPackagesPath,
            List<SourceRepository> sources,
            SourceCacheContext cacheContext,
            ILogger log)
        {
            var file = new FileInfo(inputPath);

            // Get settings relative to the input file
            var settings = _settingsProvider.GetSettings(file.DirectoryName);

            // Use settings if the global path is null
            // TODO: get effective path
            var globalPath = globalPackagesPath ?? SettingsUtility.GetGlobalPackagesFolder(settings);

            var sharedCache = _providerCache.GetOrCreate(globalPath, sources, cacheContext, log);

            var project = JsonPackageSpecReader.GetPackageSpec(file.Directory.Name, file.FullName);

            var request = new RestoreRequest(
                project,
                sharedCache,
                log,
                disposeProviders: false);

            var summaryRequest = new RestoreSummaryRequest(request, inputPath, settings, sources);

            return summaryRequest;
        }

        private static List<string> GetProjectJsonFilesInDirectory(string path)
        {
            return Directory.GetFiles(path, "*project.json", SearchOption.AllDirectories)
                .Where(file => ProjectJsonPathUtilities.IsProjectConfig(file))
                .ToList();
        }
    }
}
