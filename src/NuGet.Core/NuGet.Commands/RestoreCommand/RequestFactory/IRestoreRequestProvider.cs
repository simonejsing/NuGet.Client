using System.Collections.Generic;
using System.Threading.Tasks;
using NuGet.Logging;
using NuGet.Protocol.Core.Types;

namespace NuGet.Commands
{
    public interface IRestoreRequestProvider
    {
        /// <summary>
        /// True if this provider supports the given path. Only one provider should handle an input.
        /// </summary>
        Task<bool> Supports(string path);

        /// <summary>
        /// Create restore requests from a path.
        /// </summary>
        Task<IReadOnlyList<RestoreSummaryRequest>> CreateRequests(
            string inputPath,
            string globalPackagesPath,
            List<SourceRepository> sources,
            SourceCacheContext cacheContext,
            ILogger log);
    }
}
