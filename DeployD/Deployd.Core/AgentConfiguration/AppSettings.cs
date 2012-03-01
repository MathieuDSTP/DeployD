using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Deployd.Core.AgentConfiguration
{
    public class AppSettings : Dictionary<string,string>, IAgentSettings
    {
        public int PackageSyncIntervalMs
        {
            get { return Int32.Parse(this["PackageSyncIntervalMs"]); }
        }

        public int ConfigurationSyncIntervalMs
        {
            get { return Int32.Parse(this["ConfigurationSyncIntervalMs"]); }
        }

        public string DeploymentEnvironment
        {
            get { return this["DeploymentEnvironment"]; }
        }

        public string InstallationDirectory
        {
            get { return this["InstallationDirectory"]; }
        }

        public string NuGetRepository
        {
            get { return this["NuGetRepository"]; }
        }

        public string[] Tags
        {
            get { return this["Tags"].ToLower().Split(' ', ',', ';'); }
        }

        public string UnpackingLocation
        {
            get { return this["UnpackingLocation"]; }
        }
    }
}