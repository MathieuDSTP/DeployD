using System.Collections.Generic;
using System.Linq;
using Deployd.Agent.Services.AgentConfiguration;
using Deployd.Core.AgentConfiguration;
using Deployd.Core.PackageCaching;
using NuGet;

namespace Deployd.Agent.Services.PackageDownloading
{
    public class AllPackagesList : List<IPackage>, IPackagesList
    {
        private readonly IAgentConfigurationManager _agentConfiguration;
        private readonly IAgentSettingsManager _agentSettings;

        public AllPackagesList(IAgentConfigurationManager agentConfiguration, IAgentSettingsManager agentSettings)
        {
            _agentConfiguration = agentConfiguration;
            _agentSettings = agentSettings;
        }

        public IEnumerable<IPackage> GetWatched()
        {
            var watchedPackages = _agentConfiguration.GetWatchedPackages(_agentSettings.Settings.DeploymentEnvironment);
            return this.Where(p => watchedPackages.Any(watched => watched.Name == p.Id));
        }
    }
}