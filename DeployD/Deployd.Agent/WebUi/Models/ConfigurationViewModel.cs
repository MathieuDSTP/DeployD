using Deployd.Core.AgentConfiguration;

namespace Deployd.Agent.WebUi.Models
{
    public class ConfigurationViewModel
    {
        public IAgentWatchList WatchList { get; set; }

        public IAgentSettings Settings { get; set; }
    }
}