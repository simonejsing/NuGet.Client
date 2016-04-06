﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NuGet.ProjectManagement;
using EnvDTEProject = EnvDTE.Project;
using ThreadHelper = Microsoft.VisualStudio.Shell.ThreadHelper;
#if VS14
using NuGetVS = NuGet.VisualStudio.Proxy;
using EnvDTE.Proxy;
using Microsoft.VisualStudio.Proxy;
using Microsoft.VisualStudio.Shell.Interop.Proxy;
using Microsoft.VisualStudio.ProjectSystem.Proxy;
using Microsoft.VisualStudio.ProjectSystem.Designers.Proxy;
using MsBuildProject = Microsoft.Build.Evaluation.Proxy.ProxyProject;
using NuGet.VisualStudio.Proxy;
#else
using NuGetVS = NuGet.VisualStudio12;
#endif

namespace NuGet.PackageManagement.VisualStudio
{
    public abstract class CpsProjectSystem : VSMSBuildNuGetProjectSystem
    {
        protected CpsProjectSystem(EnvDTEProject envDTEProject, INuGetProjectContext nuGetProjectContext)
            : base(envDTEProject, nuGetProjectContext)
        {
        }

        protected override void AddGacReference(string name)
        {
            // Native & JS projects don't know about GAC
        }

        public override void AddImport(string targetFullPath, ImportLocation location)
        {
            // For VS 2012 or above, the operation has to be done inside the Writer lock
            if (String.IsNullOrEmpty(targetFullPath))
            {
                throw new ArgumentNullException(nameof(targetFullPath));
            }

            ThreadHelper.JoinableTaskFactory.Run(async delegate
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    var root = EnvDTEProjectUtility.GetFullPath(EnvDTEProject);
                    string relativeTargetPath = PathUtility.GetRelativePath(PathUtility.EnsureTrailingSlash(root), targetFullPath);
                    await AddImportStatementForVS2013Async(location, relativeTargetPath);
                });
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private async Task AddImportStatementForVS2013Async(ImportLocation location, string relativeTargetPath)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            // TODO: remove these wrappers when the proxy is present along the whole chain--we'll insert it here as a temp measure
            ProxyProject proxyProject = new ProxyProject(EnvDTEProject);
            await DoWorkInWriterLockAsync(
                proxyProject,
                new ProxyVsHierarchy(VsHierarchyUtility.ToVsHierarchy(EnvDTEProject)),
                buildProject => MicrosoftBuildEvaluationProjectUtility.AddImportStatement(buildProject.InnerInstance, relativeTargetPath, location));

            // notify the project system of the change
            UpdateImportStamp(EnvDTEProject);
        }

        public override void RemoveImport(string targetFullPath)
        {
            if (String.IsNullOrEmpty(targetFullPath))
            {
                throw new ArgumentNullException(nameof(targetFullPath), CommonResources.Argument_Cannot_Be_Null_Or_Empty);
            }

            ThreadHelper.JoinableTaskFactory.Run(async delegate
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    var root = EnvDTEProjectUtility.GetFullPath(EnvDTEProject);
                    // For VS 2012 or above, the operation has to be done inside the Writer lock
                    string relativeTargetPath = PathUtility.GetRelativePath(PathUtility.EnsureTrailingSlash(root), targetFullPath);
                    await RemoveImportStatementForVS2013Async(relativeTargetPath);
                });
        }

        // IMPORTANT: The NoInlining is required to prevent CLR from loading VisualStudio12.dll assembly while running 
        // in VS2010 and VS2012
        [MethodImpl(MethodImplOptions.NoInlining)]
        private async Task RemoveImportStatementForVS2013Async(string relativeTargetPath)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            // TODO: remove these wrappers when the proxy is present along the whole chain--we'll insert it here as a temp measure
            ProxyProject proxyProject = new ProxyProject(EnvDTEProject);
            await DoWorkInWriterLockAsync(
                proxyProject,
                new ProxyVsHierarchy(VsHierarchyUtility.ToVsHierarchy(EnvDTEProject)),
                buildProject => MicrosoftBuildEvaluationProjectUtility.RemoveImportStatement(buildProject.InnerInstance, relativeTargetPath));

            // notify the project system of the change
            UpdateImportStamp(EnvDTEProject);
        }

        public static async Task DoWorkInWriterLockAsync(Project project, IVsHierarchy hierarchy, Action<MsBuildProject> action)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var vsProject = hierarchy.As<IVsProject>();
            UnconfiguredProject unconfiguredProject = GetUnconfiguredProject(vsProject);
            if (unconfiguredProject != null)
            {
                var service = unconfiguredProject.ProjectService_proxy.Services_proxy.ProjectLockService_proxy;
                if (service != null)
                {
                    using (ProjectWriteLockReleaser x = await service.WriteLockAsync_proxy())
                    {
                        await x.CheckoutAsync(unconfiguredProject.FullPath);

                        // TODO - originally:
                        //
                        // ConfiguredProject configuredProject = await unconfiguredProject.GetSuggestedConfiguredProjectAsync();
                        //
                        // Change this to make less of a proxy footprint in code
                        ConfiguredProject configuredProject = (ConfiguredProject)await Utility.TaskCast<ProxyConfiguredProject>(
                            () => unconfiguredProject.GetSuggestedConfiguredProjectAsync());

                        MsBuildProject buildProject = await x.GetProjectAsync(configuredProject);

                        if (buildProject != null)
                        {
                            action(buildProject);
                        }

                        await x.ReleaseAsync();
                    }

                    await unconfiguredProject.ProjectService.Services.ThreadingPolicy.SwitchToUIThread();
                    project.Save();
                }
            }
        }

        private static UnconfiguredProject GetUnconfiguredProject(IVsProject project)
        {
            IVsBrowseObjectContext context = project.As<IVsBrowseObjectContext>();
            if (context == null)
            {
                IVsHierarchy hierarchy = project as IVsHierarchy;
                if (hierarchy != null)
                {
                    object extObject;
                    if (ErrorHandler.Succeeded(hierarchy.GetProperty((uint)VSConstants.VSITEMID.Root, (int)__VSHPROPID.VSHPROPID_ExtObject, out extObject)))
                    {
                        Project dteProject = extObject as Project;
                        if (dteProject != null)
                        {
                            context = dteProject.Object as IVsBrowseObjectContext;
                        }
                    }
                }
            }

            return context != null ? (UnconfiguredProject)context.UnconfiguredProject : null;
        }
    }
}
