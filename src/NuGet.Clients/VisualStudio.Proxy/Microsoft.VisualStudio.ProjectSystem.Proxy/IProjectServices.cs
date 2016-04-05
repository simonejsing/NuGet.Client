// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.ProjectSystem.Proxy
{
    public interface IProjectServices : global::Microsoft.VisualStudio.ProjectSystem.IProjectServices
    {
        ProxyProjectLockService ProjectLockService_proxy { get; }
    }
}
