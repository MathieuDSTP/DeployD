using System;
using System.IO.Abstractions;
using Raven.Client;

namespace Deployd.Core.AgentConfiguration
{
    public class DocumentStoreSettingsStore : IAgentSettingsStore
    {
        private readonly IDocumentSession _session;
        private readonly IFileSystem _fileSystem;

        public DocumentStoreSettingsStore(IDocumentSession session, IFileSystem fileSystem)
        {
            _session = session;
            _fileSystem = fileSystem;
        }

        private AgentSettings Defaults
        {
            get
            {
                return new AgentSettings()
                    {
                        Id="AgentSettings/1",
                        NuGetRepository = _fileSystem.MapVirtualPath("~\\DebugPackageSource"),
                        UnpackingLocation = _fileSystem.MapVirtualPath("~\\app_unpack"),
                        InstallationDirectory = _fileSystem.MapVirtualPath("~\\app_root"),
                        DeploymentEnvironment = "Production",
                        ConfigurationSyncIntervalMs = 60000,
                        PackageSyncIntervalMs = 60000,
                        LatestDirectory = _fileSystem.MapVirtualPath("~\\latest"),
                        CacheDirectory = _fileSystem.MapVirtualPath("~\\package_cache"),
                        BaseInstallationPath = "c:\\installations",
                        MsDeployServiceUrl = "localhost",
                        Tags = new string[0],
                        LogsDirectory = _fileSystem.MapVirtualPath("~\\logs"),
                        HubAddress = "http://localhost:80",
                        MaxConcurrentInstallations = 3,
                        EnableConfigurationSync = false,
                        XMPPSettings = new XmppSettings()
                            {
                                Enabled = false,
                                Host="talk.google.com",
                                Port = 5222,

                            },
                            NotificationRecipients=""
                    };
            }
        }

        public IAgentSettings LoadSettings()
        {
            var settings = _session.Load<AgentSettings>("AgentSettings/1");
            if (settings == null)
            {
                settings = Defaults;
                _session.Store(settings);
                _session.SaveChanges();
            }

            return settings;
        }

        public event EventHandler SettingsChanged;
        public void UpdateSettings(dynamic settingsValues)
        {
            var settings = _session.Load<AgentSettings>("AgentSettings/1");
            if (settings == null)
            {
                settings = Defaults;
            }
            settings.HubAddress = settingsValues.HubAddress;
            settings.BaseInstallationPath = settingsValues.BaseInstallationPath;
            settings.NuGetRepository = settingsValues.NuGetRepository;
            settings.DeploymentEnvironment = settingsValues.DeploymentEnvironment;
            settings.NotificationRecipients = settingsValues.NotificationRecipients;

            settings.PackageSyncIntervalMs = settingsValues.PackageSyncIntervalMs;
            settings.ConfigurationSyncIntervalMs = settingsValues.ConfigurationSyncIntervalMs;
            settings.MaxConcurrentInstallations = settingsValues.MaxConcurrentInstallations;

            _session.SaveChanges();

            if (SettingsChanged != null)
                SettingsChanged(this, new EventArgs());
        }
    }
}