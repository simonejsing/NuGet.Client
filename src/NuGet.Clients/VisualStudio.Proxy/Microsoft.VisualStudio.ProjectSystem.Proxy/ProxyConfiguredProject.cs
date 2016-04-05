// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.VisualStudio.Threading;
using NuGet.VisualStudio.Proxy;

namespace Microsoft.VisualStudio.ProjectSystem.Proxy
{
    [InnerType(typeof(global::Microsoft.VisualStudio.ProjectSystem.ConfiguredProject))]
    public class ProxyConfiguredProject : VsProxy, global::Microsoft.VisualStudio.ProjectSystem.ConfiguredProject
    {
        public ProxyConfiguredProject() { }

        public ProxyConfiguredProject(global::Microsoft.VisualStudio.ProjectSystem.ConfiguredProject configuredProject) :
            base(configuredProject)
        { }

        internal global::Microsoft.VisualStudio.ProjectSystem.ConfiguredProject Instance
        {
            get { return (global::Microsoft.VisualStudio.ProjectSystem.ConfiguredProject)_instance; }
        }

        #region PassThroughs
        public System.Collections.Immutable.IImmutableSet<string> Capabilities
        {
            get { return Instance.Capabilities; }
        }

        public ProjectConfiguration ProjectConfiguration
        {
            get { return Instance.ProjectConfiguration; }
        }

        public IComparable ProjectVersion
        {
            get { return Instance.ProjectVersion; }
        }

        public System.Threading.Tasks.Dataflow.IReceivableSourceBlock<IComparable> ProjectVersionBlock
        {
            get { return Instance.ProjectVersionBlock; }
        }

        public IConfiguredProjectServices Services
        {
            get { return Instance.Services; }
        }

        public ProjectSystem.UnconfiguredProject UnconfiguredProject
        {
            get { return Instance.UnconfiguredProject; }
        }

        public IComparable Version
        {
            get { return Instance.Version; }
        }

        // TODO wrap these
        public event EventHandler Changed;
        public event EventHandler ProjectChanged;
        public event EventHandler ProjectChangedSynchronous;
        public event AsyncEventHandler ProjectUnloading;

        public bool IsProjectCapabilityPresent(string projectCapability)
        {
            return Instance.IsProjectCapabilityPresent(projectCapability);
        }
        #endregion
    }
}
