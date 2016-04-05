// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using NuGet.VisualStudio.Proxy;

namespace Microsoft.VisualStudio.ProjectSystem.Proxy
{
    public struct ProjectWriteLockAwaitable
    {
        private global::Microsoft.VisualStudio.ProjectSystem.ProjectWriteLockAwaitable _instance;

        public ProjectWriteLockAwaitable(global::Microsoft.VisualStudio.ProjectSystem.ProjectWriteLockAwaitable instance)
        {
            _instance = instance;
        }

        public ProjectWriteLockAwaiter GetAwaiter()
        {
            return new ProjectWriteLockAwaiter(_instance.GetAwaiter());
        }
    }

    public struct ProjectWriteLockAwaiter : System.Runtime.CompilerServices.INotifyCompletion
    {
        private global::Microsoft.VisualStudio.ProjectSystem.ProjectWriteLockAwaiter _instance;

        public ProjectWriteLockAwaiter(global::Microsoft.VisualStudio.ProjectSystem.ProjectWriteLockAwaiter instance)
        {
            _instance = instance;
        }

        public bool IsCompleted
        {
            get { return _instance.IsCompleted; }
        }

        public ProjectWriteLockReleaser GetResult()
        {
            return new ProjectWriteLockReleaser(_instance.GetResult());
        }

        public void OnCompleted(Action continuation)
        {
            _instance.OnCompleted(continuation);
        }
    }

    public struct ProjectWriteLockReleaser : IDisposable
    {
        private global::Microsoft.VisualStudio.ProjectSystem.ProjectWriteLockReleaser _instance;

        public ProjectWriteLockReleaser(global::Microsoft.VisualStudio.ProjectSystem.ProjectWriteLockReleaser instance)
        {
            _instance = instance;
        }

        public void Dispose()
        {
            _instance.Dispose();
        }

        public Task CheckoutAsync(string file)
        {
            return _instance.CheckoutAsync(file);
        }

        public Task<Microsoft.Build.Evaluation.Proxy.ProxyProject> GetProjectAsync(global::Microsoft.VisualStudio.ProjectSystem.ConfiguredProject configuredProject)
        {
            if (configuredProject is ProxyConfiguredProject)
            {
                global::Microsoft.VisualStudio.ProjectSystem.ProjectWriteLockReleaser instance = _instance;
                return NuGet.VisualStudio.Proxy.Utility.TaskCast<Microsoft.Build.Evaluation.Proxy.ProxyProject>(
                    () => instance.GetProjectAsync((configuredProject as ProxyConfiguredProject).Instance));
            }
            return null;
        }

        public Task ReleaseAsync()
        {
            return _instance.ReleaseAsync();
        }
    }

    [InnerType(typeof(global::Microsoft.VisualStudio.ProjectSystem.IProjectLockService))]
    public class ProxyProjectLockService : VsProxy, global::Microsoft.VisualStudio.ProjectSystem.IProjectLockService
    {
        internal ProxyProjectLockService() { }

        public ProxyProjectLockService(global::Microsoft.VisualStudio.ProjectSystem.IProjectLockService projectLockService) :
            base(projectLockService)
        {
        }

        private global::Microsoft.VisualStudio.ProjectSystem.IProjectLockService Instance
        {
            get { return (global::Microsoft.VisualStudio.ProjectSystem.IProjectLockService)_instance; }
        }

        #region PassThroughs
        public bool IsAnyLockHeld
        {
            get { return Instance.IsAnyLockHeld; }
        }

        public bool IsAnyPassiveLockHeld
        {
            get { return Instance.IsAnyPassiveLockHeld; }
        }

        public bool IsReadLockHeld
        {
            get { return Instance.IsReadLockHeld; }
        }

        public bool IsPassiveReadLockHeld
        {
            get { return Instance.IsPassiveReadLockHeld; }
        }

        public bool IsUpgradeableReadLockHeld
        {
            get { return Instance.IsUpgradeableReadLockHeld; }
        }

        public bool IsPassiveUpgradeableReadLockHeld
        {
            get { return Instance.IsPassiveUpgradeableReadLockHeld; }
        }

        public bool IsWriteLockHeld
        {
            get { return Instance.IsWriteLockHeld; }
        }

        public bool IsPassiveWriteLockHeld
        {
            get { return Instance.IsPassiveWriteLockHeld; }
        }

        public ProjectLockAwaitable ReadLockAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Instance.ReadLockAsync(cancellationToken);
        }

        public ProjectLockAwaitable UpgradeableReadLockAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Instance.UpgradeableReadLockAsync(cancellationToken);
        }

        public ProjectLockAwaitable UpgradeableReadLockAsync(ProjectLockFlags options, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Instance.UpgradeableReadLockAsync(options, cancellationToken);
        }

        public ProjectWriteLockAwaitable WriteLockAsync_proxy(CancellationToken cancellationToken = default(CancellationToken))
        {
            return new ProjectWriteLockAwaitable(Instance.WriteLockAsync(cancellationToken));
        }

        public global::Microsoft.VisualStudio.ProjectSystem.ProjectWriteLockAwaitable WriteLockAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Instance.WriteLockAsync(cancellationToken);
        }

        public global::Microsoft.VisualStudio.ProjectSystem.ProjectWriteLockAwaitable WriteLockAsync(ProjectLockFlags options, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Instance.WriteLockAsync(options, cancellationToken);
        }

        public ProjectLockSuppression HideLocks()
        {
            return Instance.HideLocks();
        }
        #endregion
    }
}
