// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Threading;
using NuGet.VisualStudio.Proxy;

namespace Microsoft.VisualStudio.ProjectSystem.Proxy
{
    [InnerType(typeof(global::Microsoft.VisualStudio.ProjectSystem.UnconfiguredProject))]
    public class ProxyUnconfiguredProject : VsProxy, global::Microsoft.VisualStudio.ProjectSystem.UnconfiguredProject
    {
        internal ProxyUnconfiguredProject() { }

        public ProxyUnconfiguredProject(global::Microsoft.VisualStudio.ProjectSystem.UnconfiguredProject unconfiguredProject) :
            base(unconfiguredProject)
        { }

        private global::Microsoft.VisualStudio.ProjectSystem.UnconfiguredProject Instance
        {
            get { return (global::Microsoft.VisualStudio.ProjectSystem.UnconfiguredProject)_instance; }
        }

        #region PassThroughs
        public System.Collections.Immutable.IImmutableSet<string> Capabilities
        {
            get { return Instance.Capabilities; }
        }

        public string FullPath
        {
            get { return Instance.FullPath; }
        }

        public IEnumerable<global::Microsoft.VisualStudio.ProjectSystem.ConfiguredProject> LoadedConfiguredProjects
        {
            get { return Instance.LoadedConfiguredProjects; }
        }

        public global::Microsoft.VisualStudio.ProjectSystem.ProjectService ProjectService
        {
            get { return new ProxyProjectService(Instance.ProjectService); }
        }

        public ProxyProjectService ProjectService_proxy
        {
            get { return new ProxyProjectService(Instance.ProjectService); }
        }

        public bool RequiresReloadForExternalFileChange
        {
            get { return Instance.RequiresReloadForExternalFileChange; }
        }

        public IUnconfiguredProjectServices Services
        {
            get { return Instance.Services; }
        }

        public IComparable Version
        {
            get { return Instance.Version; }
        }

        // TODO fire events on inner instance
        public event EventHandler Changed;
        public event AsyncEventHandler<ProjectRenamedEventArgs> ProjectRenamed;
        public event AsyncEventHandler<ProjectRenamedEventArgs> ProjectRenamedOnWriter;
        public event AsyncEventHandler<ProjectRenamedEventArgs> ProjectRenaming;
        public event AsyncEventHandler ProjectUnloading;

        public Task<bool> CanRenameAsync(string newFilePath = null)
        {
            return Instance.CanRenameAsync(newFilePath);
        }

        public Task<Encoding> GetFileEncodingAsync()
        {
            return Instance.GetFileEncodingAsync();
        }

        public Task<bool> GetIsDirtyAsync()
        {
            return Instance.GetIsDirtyAsync();
        }

        public Task<global::Microsoft.VisualStudio.ProjectSystem.ConfiguredProject> GetSuggestedConfiguredProjectAsync()
        {
            var tcs = new TaskCompletionSource<global::Microsoft.VisualStudio.ProjectSystem.ConfiguredProject>();
            Instance.GetSuggestedConfiguredProjectAsync().ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    tcs.TrySetException(t.Exception.InnerException);
                }
                else if (t.IsCanceled)
                {
                    tcs.TrySetCanceled();
                }
                else
                {
                    tcs.TrySetResult(new ProxyConfiguredProject(t.Result));
                }
            }, TaskContinuationOptions.ExecuteSynchronously);
            return tcs.Task;
        }

        public bool IsProjectCapabilityPresent(string projectCapability)
        {
            return Instance.IsProjectCapabilityPresent(projectCapability);
        }

        public Task<global::Microsoft.VisualStudio.ProjectSystem.ConfiguredProject> LoadConfiguredProjectAsync(ProjectConfiguration projectConfiguration)
        {
            return Instance.LoadConfiguredProjectAsync(projectConfiguration);
        }

        public Task<global::Microsoft.VisualStudio.ProjectSystem.ConfiguredProject> LoadConfiguredProjectAsync(string name, System.Collections.Immutable.IImmutableDictionary<string, string> configurationProperties)
        {
            return Instance.LoadConfiguredProjectAsync(name, configurationProperties);
        }

        public Task RenameAsync(string newFilePath)
        {
            return Instance.RenameAsync(newFilePath);
        }

        public Task SaveAsync(string filePath = null)
        {
            return Instance.SaveAsync(filePath);
        }

        public Task SaveCopyAsync(string filePath, Encoding fileEncoding = null)
        {
            return Instance.SaveCopyAsync(filePath, fileEncoding);
        }

        public Task SaveUserFileAsync()
        {
            return Instance.SaveUserFileAsync();
        }

        public Task SetFileEncodingAsync(Encoding value)
        {
            return Instance.SetFileEncodingAsync(value);
        }
        #endregion
    }
}
