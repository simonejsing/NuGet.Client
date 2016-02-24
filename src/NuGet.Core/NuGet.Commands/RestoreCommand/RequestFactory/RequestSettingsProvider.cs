using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NuGet.Configuration;

namespace NuGet.Commands
{
    public class RequestSettingsProvider
    {
        public string ConfigFileName { get; set; }

        public IMachineWideSettings MachineWideSettings { get; set; }

        public ISettings GetSettings(string projectDirectory)
        {
            return Settings.LoadDefaultSettings(projectDirectory,
                ConfigFileName,
                MachineWideSettings);
        }
    }
}
