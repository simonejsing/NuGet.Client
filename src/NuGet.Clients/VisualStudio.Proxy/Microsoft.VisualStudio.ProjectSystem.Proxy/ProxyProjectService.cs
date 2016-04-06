﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using NuGet.VisualStudio.Proxy;

namespace Microsoft.VisualStudio.ProjectSystem.Proxy
{
    [InnerType(typeof(global::Microsoft.VisualStudio.ProjectSystem.ProjectService))]
    public class ProxyProjectService : VsProxy, global::Microsoft.VisualStudio.ProjectSystem.ProjectService
    {
        internal ProxyProjectService() { }

        public ProxyProjectService(global::Microsoft.VisualStudio.ProjectSystem.ProjectService projectService) :
            base(projectService)
        {
        }

        private global::Microsoft.VisualStudio.ProjectSystem.ProjectService Instance
        {
            get { return (global::Microsoft.VisualStudio.ProjectSystem.ProjectService)_instance; }
        }

        #region PassThroughs
        public IEnumerable<ProjectSystem.UnconfiguredProject> LoadedUnconfiguredProjects
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public global::Microsoft.VisualStudio.ProjectSystem.IProjectServices Services
        {
            get { return new ProxyProjectServices(Instance.Services); }
        }

        public ProxyProjectServices Services_proxy
        {
            get { return new ProxyProjectServices(Instance.Services); }
        }

        public IImmutableSet<string> ServiceCapabilities
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IComparable Version
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public event EventHandler Changed;

        public Task<ProjectSystem.UnconfiguredProject> LoadProjectAsync(string projectLocation, IImmutableSet<string> projectCapabilities = null)
        {
            throw new NotImplementedException();
        }

        public Task<ProjectSystem.UnconfiguredProject> LoadProjectAsync(System.Xml.XmlReader reader, IImmutableSet<string> projectCapabilities = null)
        {
            throw new NotImplementedException();
        }

        public Task UnloadProjectAsync(ProjectSystem.UnconfiguredProject project)
        {
            throw new NotImplementedException();
        }

        public bool IsProjectCapabilityPresent(string projectCapability)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
