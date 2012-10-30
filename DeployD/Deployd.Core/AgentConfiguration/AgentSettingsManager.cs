using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using ILogger = Ninject.Extensions.Logging.ILogger;

namespace Deployd.Core.AgentConfiguration
{
    public class AgentSettingsManager : IAgentSettingsManager
    {
        public AgentSettingsManager(IAgentSettingsStore agentSettingsStore, ILogger log)
        {
            _agentSettingsStore = agentSettingsStore;
            _log = log;
            _agentSettingsStore.SettingsChanged += (sender, args) => LoadSettings();
        }

        private readonly ILogger _log;
        public static Dictionary<string, string> ConfigurationDefaults { get; private set; }
        private IAgentSettings _settings = null;
        private readonly IAgentSettingsStore _agentSettingsStore;

        public IAgentSettings Settings
        {
            get
            {
                if (_settings == null)
                    _settings = LoadSettings();

                return _settings;
            }
        }

        public void UnloadSettings()
        {
            _settings = null;
        }

        public void UpdateSettings(dynamic settingsValues)
        {
            _agentSettingsStore.UpdateSettings(settingsValues);
        }

        static AgentSettingsManager()
        {
            ConfigurationDefaults = new Dictionary<string, string>
            {
                {"NuGetRepository", "~\\DebugPackageSource"},
                {"UnpackingLocation", "~\\app_unpack"},
                {"InstallationDirectory", "~\\app_root"},
                {"DeploymentEnvironment", "Production"},
                {"ConfigurationSyncIntervalMs", "60000"},
                {"PackageSyncIntervalMs", "60000"},
                {"LatestDirectory", "~\\latest"},
                {"CacheDirectory", "~\\package_cache"},
                {"BaseInstallationPath", "c:\\installations"},
                {"MsDeployServiceUrl", "localhost"},
                {"Tags",""},
                {"LogsDirectory", "~\\logs"},
                {"Hub.Address", "http://localhost:80"},
                {"MaxConcurrentInstallations", "3"},
                {"EnableConfigurationSync", "false"},
                {"Notifications.XMPP.Enabled","false"},
                {"Notifications.XMPP.Host","talk.google.com"},
                {"Notifications.XMPP.Port","5222"},
                {"Notifications.XMPP.Username",""},
                {"Notifications.XMPP.Password",""},
                {"Notifications.Recipients", ""},
                
            };
        }

        public IAgentSettings LoadSettings()
        {
            _log.Debug("Loading settings from {0}", _agentSettingsStore.GetType().Name);


            return _agentSettingsStore.LoadSettings();
        }
    }
}
