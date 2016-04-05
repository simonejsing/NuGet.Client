// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using NuGet.VisualStudio.Proxy;

namespace Microsoft.VisualStudio.Shell.Interop.Proxy
{
    [InnerType(typeof(global::Microsoft.VisualStudio.Shell.Interop.IVsProject))]
    public class VsProjectProxy : NuGet.VisualStudio.Proxy.VsProxy, global::Microsoft.VisualStudio.Shell.Interop.IVsProject
    {
        internal VsProjectProxy() { }

        public VsProjectProxy(global::Microsoft.VisualStudio.Shell.Interop.IVsProject vsProject)
            : base(vsProject)
        { }

        private global::Microsoft.VisualStudio.Shell.Interop.IVsProject Instance
        {
            get { return (global::Microsoft.VisualStudio.Shell.Interop.IVsProject)_instance; }
        }

        #region PassThroughs

        // TODO: make abstract parameters that allow proxies and non-proxies

        public int AddItem(uint itemidLoc, VSADDITEMOPERATION dwAddItemOperation, string pszItemName, uint cFilesToOpen, string[] rgpszFilesToOpen, IntPtr hwndDlgOwner, VSADDRESULT[] pResult)
        {
            return Instance.AddItem(itemidLoc, dwAddItemOperation, pszItemName, cFilesToOpen, rgpszFilesToOpen, hwndDlgOwner, pResult);
        }

        public int GenerateUniqueItemName(uint itemidLoc, string pszExt, string pszSuggestedRoot, out string pbstrItemName)
        {
            return Instance.GenerateUniqueItemName(itemidLoc, pszExt, pszSuggestedRoot, out pbstrItemName);
        }

        public int GetItemContext(uint itemid, out global::Microsoft.VisualStudio.OLE.Interop.IServiceProvider ppSP)
        {
            return Instance.GetItemContext(itemid, out ppSP);
        }

        public int GetMkDocument(uint itemid, out string pbstrMkDocument)
        {
            return Instance.GetMkDocument(itemid, out pbstrMkDocument);
        }

        public int IsDocumentInProject(string pszMkDocument, out int pfFound, VSDOCUMENTPRIORITY[] pdwPriority, out uint pitemid)
        {
            return Instance.IsDocumentInProject(pszMkDocument, out pfFound, pdwPriority, out pitemid);
        }

        public int OpenItem(uint itemid, ref Guid rguidLogicalView, IntPtr punkDocDataExisting, out IVsWindowFrame ppWindowFrame)
        {
            return Instance.OpenItem(itemid, ref rguidLogicalView, punkDocDataExisting, out ppWindowFrame);
        }
        #endregion
    }
}
