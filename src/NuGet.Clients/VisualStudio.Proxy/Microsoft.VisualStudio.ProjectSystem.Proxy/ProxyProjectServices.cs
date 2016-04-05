// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.VisualStudio.ProjectSystem.Designers;
using NuGet.VisualStudio.Proxy;

namespace Microsoft.VisualStudio.ProjectSystem.Proxy
{
    [InnerType(typeof(global::Microsoft.VisualStudio.ProjectSystem.IProjectServices))]
    public class ProxyProjectServices : VsProxy, global::Microsoft.VisualStudio.ProjectSystem.IProjectServices
    {
        internal ProxyProjectServices() { }

        public ProxyProjectServices(global::Microsoft.VisualStudio.ProjectSystem.IProjectServices projectServices) :
            base(projectServices)
        {
        }

        private global::Microsoft.VisualStudio.ProjectSystem.IProjectServices Instance
        {
            get { return (global::Microsoft.VisualStudio.ProjectSystem.IProjectServices)_instance; }
        }

        #region PassThroughs
        public global::Microsoft.VisualStudio.ProjectSystem.IProjectLockService ProjectLockService
        {
            get { return new ProxyProjectLockService(Instance.ProjectLockService); }
        }

        public ProxyProjectLockService ProjectLockService_proxy
        {
            get { return new ProxyProjectLockService(Instance.ProjectLockService); }
        }

        public IThreadHandling ThreadingPolicy
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IProjectFaultHandlerService FaultHandler
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IProjectReloader ProjectReloader
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public VisualStudio.Composition.ExportProvider ExportProvider
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        #endregion
    }
}
