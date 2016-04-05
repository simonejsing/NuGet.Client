// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using NuGet.VisualStudio.Proxy;

namespace Microsoft.VisualStudio.Shell.Interop.Proxy
{
    public enum __VSHPROPID
    {
        VSHPROPID_ExtObject = -2027,
    }

    public interface IVsHierarchy : IVsProxy, global::Microsoft.VisualStudio.Shell.Interop.IVsHierarchy { }
}
