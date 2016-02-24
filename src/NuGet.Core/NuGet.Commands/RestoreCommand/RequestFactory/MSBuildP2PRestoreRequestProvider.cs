using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.ProjectModel;

namespace NuGet.Commands
{
    public class MSBuildP2PRestoreRequestProvider : IRestoreRequestProvider
    {
        private readonly RestoreCommandProvidersCache _providerCache;

        public MSBuildP2PRestoreRequestProvider(RestoreCommandProvidersCache providerCache)
        {
            _providerCache = providerCache;
        }

        public Task<IReadOnlyList<RestoreSummaryRequest>> CreateRequests(
            string inputPath,
            RestoreArgs restoreContext)
        {
            var paths = new List<string>();
            var requests = new List<RestoreSummaryRequest>();
            var rootPath = Path.GetDirectoryName(inputPath);

            // Get settings relative to the input file
            var settings = restoreContext.GetSettings(rootPath);

            var globalPath = restoreContext.GetEffectiveGlobalPackagesFolder(rootPath, settings);

            var lines = File.ReadAllLines(inputPath);
            var msbuildProvider = new MSBuildProjectReferenceProvider(lines);

            var entryPoints = msbuildProvider.GetEntryPoints();

            // Create a request for each top level project with project.json
            foreach (var entryPoint in entryPoints)
            {
                if (entryPoint.PackageSpecPath != null && entryPoint.MSBuildProjectPath != null)
                {
                    var request = Create(
                        globalPath,
                        settings,
                        entryPoint,
                        msbuildProvider,
                        restoreContext);

                    requests.Add(request);
                }
            }

            return Task.FromResult<IReadOnlyList<RestoreSummaryRequest>>(requests);
        }

        public Task<bool> Supports(string path)
        {
            // True if dir or project.json file
            var result = (File.Exists(path) && path.EndsWith(".msbuildp2p", StringComparison.OrdinalIgnoreCase));

            return Task.FromResult(result);
        }

        private RestoreSummaryRequest Create(
            string globalPath,
            ISettings settings,
            ExternalProjectReference project,
            MSBuildProjectReferenceProvider msbuildProvider,
            RestoreArgs restoreContext)
        {
            var sharedCache = _providerCache.GetOrCreate(
                globalPath,
                restoreContext.Sources,
                restoreContext.CacheContext,
                restoreContext.Log);

            var request = new RestoreRequest(
                project.PackageSpec,
                sharedCache,
                restoreContext.Log,
                disposeProviders: false);

            request.MaxDegreeOfConcurrency =
                restoreContext.DisableParallel ? 1 : RestoreRequest.DefaultDegreeOfConcurrency;

            request.ExternalProjects = msbuildProvider.GetReferences(project.MSBuildProjectPath).ToList();

            var summaryRequest = new RestoreSummaryRequest(
                request,
                project.MSBuildProjectPath,
                settings,
                restoreContext.Sources);

            return summaryRequest;
        }
    }
}
