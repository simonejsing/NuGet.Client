// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using NuGet.VisualStudio.Proxy;
using Microsoft.VisualStudio.ProjectSystem.Properties;

namespace Microsoft.VisualStudio.ProjectSystem.Designers.Proxy
{
    [InnerType(typeof(global::Microsoft.VisualStudio.ProjectSystem.Designers.IVsBrowseObjectContext))]
    public class ProxyVsBrowseObjectContext : VsProxy, global::Microsoft.VisualStudio.ProjectSystem.Designers.IVsBrowseObjectContext
    {
        internal ProxyVsBrowseObjectContext() { }

        public ProxyVsBrowseObjectContext(global::Microsoft.VisualStudio.ProjectSystem.Designers.IVsBrowseObjectContext vsBrowseObjectContext) :
            base(vsBrowseObjectContext)
        { }

        private global::Microsoft.VisualStudio.ProjectSystem.Designers.IVsBrowseObjectContext Instance
        {
            get { return (global::Microsoft.VisualStudio.ProjectSystem.Designers.IVsBrowseObjectContext)_instance; }
        }

        #region PassThroughs
        public ConfiguredProject ConfiguredProject
        {
            get { return Instance.ConfiguredProject; }
        }

        public IProjectPropertiesContext ProjectPropertiesContext
        {
            get { return Instance.ProjectPropertiesContext; }
        }

        public IPropertySheet PropertySheet
        {
            get { return Instance.PropertySheet; }
        }

        public global::Microsoft.VisualStudio.ProjectSystem.UnconfiguredProject UnconfiguredProject
        {
            get { return new Microsoft.VisualStudio.ProjectSystem.Proxy.ProxyUnconfiguredProject(Instance.UnconfiguredProject); }
        }
        #endregion
    }
}
