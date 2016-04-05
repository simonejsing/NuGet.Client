// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using NuGet.VisualStudio.Proxy;

namespace EnvDTE.Proxy
{
    [InnerType(typeof(global::EnvDTE.Project))]
    public class ProxyProject : VsProxy, Project
    {
        // TODO: wrap returned types in proxies

        internal ProxyProject() { }

        public ProxyProject(EnvDTE.Project project) :
            base(project)
        { }

        // TODO: move these to the base class
        private global::EnvDTE.Project Instance
        {
            get { return (global::EnvDTE.Project)_instance; }
        }

        #region PassThroughs

        public CodeModel CodeModel
        {
            get { return Instance.CodeModel; }
        }

        public Projects Collection
        {
            get { return Instance.Collection; }
        }

        public ConfigurationManager ConfigurationManager
        {
            get { return Instance.ConfigurationManager; }
        }

        public DTE DTE
        {
            get { return Instance.DTE; }
        }

        public string ExtenderCATID
        {
            get { return Instance.ExtenderCATID; }
        }

        public dynamic ExtenderNames
        {
            get { return Instance.ExtenderNames; }
        }

        public string FileName
        {
            get { return Instance.FileName; }
        }

        public string FullName
        {
            get { return Instance.FullName; }
        }

        public Globals Globals
        {
            get { return Instance.Globals; }
        }

        public bool IsDirty
        {
            get { return Instance.IsDirty; }
            set { Instance.IsDirty = value; }
        }

        public string Kind
        {
            get { return Instance.Kind; }
        }

        public string Name
        {
            get { return Instance.Name; }
            set { Instance.Name = value; }
        }

        public dynamic Object
        {
            get { return Instance.Object; }
        }

        public ProjectItem ParentProjectItem
        {
            get { return Instance.ParentProjectItem; }
        }

        public ProjectItems ProjectItems
        {
            get { return Instance.ProjectItems; }
        }

        public Properties Properties
        {
            get { return Instance.Properties; }
        }

        public bool Saved
        {
            get { return Instance.Saved; }
            set { Instance.Saved = value; }
        }

        public string UniqueName
        {
            get { return Instance.UniqueName; }
        }

        public void Delete()
        {
            Instance.Delete();
        }

        public dynamic get_Extender(string ExtenderName)
        {
            return Instance.get_Extender(ExtenderName);
        }

        public void Save(string FileName = "")
        {
            Instance.Save(FileName);
        }

        public void SaveAs(string NewFileName)
        {
            Instance.SaveAs(NewFileName);
        }
        #endregion
    }
}
