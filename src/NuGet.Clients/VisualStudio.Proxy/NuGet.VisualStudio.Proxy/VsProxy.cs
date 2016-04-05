// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Linq;

namespace NuGet.VisualStudio.Proxy
{
    public abstract class VsProxy : IVsProxy
    {
        internal object _instance;

        internal VsProxy() { }

        public VsProxy(object instance)
        {
            Debug.Assert(instance != null, "A proxy instance must have an inner instance");
            _instance = instance;
        }

        public bool Is<T>() where T : class
        {
            if (typeof(T).IsSubclassOf(typeof(NuGet.VisualStudio.Proxy.VsProxy)))
            {
                Attribute[] attributes = System.Attribute.GetCustomAttributes(typeof(T));
                NuGet.VisualStudio.Proxy.InnerTypeAttribute innerTypeAttribute =
                    attributes?.FirstOrDefault(a => a is NuGet.VisualStudio.Proxy.InnerTypeAttribute)
                    as NuGet.VisualStudio.Proxy.InnerTypeAttribute;
                if (innerTypeAttribute != null)
                {
                    return innerTypeAttribute.InnerType.IsAssignableFrom(_instance.GetType());
                }
            }

            return false;
        }

        public T As<T>() where T : class
        {
            if (_instance.GetType().IsAssignableFrom(typeof(T)))
            {
                return (T)CreateVsProxy<T>(_instance);
            }

            return default(T);
        }

        /// <summary>
        /// Proxy factory method
        /// </summary>
        internal IVsProxy CreateVsProxy<T>(object instance)
        {
            Debug.Assert(instance != null, "Proxy requires an inner instance");
            if (instance == null)
            {
                return null;
            }

            if (typeof(T) == typeof(global::Microsoft.VisualStudio.ProjectSystem.Designers.IVsBrowseObjectContext))
            {
                return new Microsoft.VisualStudio.ProjectSystem.Designers.Proxy.ProxyVsBrowseObjectContext(
                    instance as global::Microsoft.VisualStudio.ProjectSystem.Designers.IVsBrowseObjectContext);
            }

            if (typeof(T) == typeof(global::Microsoft.VisualStudio.Shell.Interop.IVsProject))
            {
                return new Microsoft.VisualStudio.Shell.Interop.Proxy.VsProjectProxy(
                    instance as global::Microsoft.VisualStudio.Shell.Interop.IVsProject);
            }

            return null;
        }
    }
}
