// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.ProjectSystem.Proxy
{
    public interface UnconfiguredProject : global::Microsoft.VisualStudio.ProjectSystem.UnconfiguredProject
    {
        ProxyProjectService ProjectService_proxy { get; }
    }
}
