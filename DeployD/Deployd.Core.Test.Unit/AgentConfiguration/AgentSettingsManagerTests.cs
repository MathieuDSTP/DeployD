using System.Configuration;
using System.IO.Abstractions;
using Deployd.Core.AgentConfiguration;
using Moq;
using NUnit.Framework;
using Ninject.Extensions.Logging;

namespace Deployd.Core.Test.Unit.AgentConfiguration
{
    [TestFixture]
    public class AgentSettingsManagerTests
    {
        private AgentSettingsManager _mgr;
        private Mock<IFileSystem> _fileSystemMock;
        private Mock<ILogger> _loggerMock;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger>();

            var settingsMock = new Mock<IAgentSettingsStore>();
            settingsMock
                .Setup(x => x.LoadSettings())
                .Returns(new AgentSettings()
                {
                    ConfigurationSyncIntervalMs = 1,
                    DeploymentEnvironment = "2",
                    InstallationDirectory = "3",
                    NuGetRepository = "4",
                    PackageSyncIntervalMs = 5,
                    UnpackingLocation = "6"
                });

            _fileSystemMock = new Mock<IFileSystem>();
            _fileSystemMock.Setup(x => x.Directory.GetCurrentDirectory()).Returns("c:\\fakedirectory");
            _mgr = new AgentSettingsManager(settingsMock.Object, _loggerMock.Object);            
        }

        [Test]
        public void LoadSettings_WhenSuppliedWithConfiguration_TakesConfiguration()
        {
            var settingsManager = _mgr.LoadSettings();

            Assert.That(settingsManager.ConfigurationSyncIntervalMs, Is.EqualTo(1));
            Assert.That(settingsManager.DeploymentEnvironment, Is.EqualTo("2"));
            Assert.That(settingsManager.InstallationDirectory, Is.StringContaining("3"));
            Assert.That(settingsManager.NuGetRepository, Is.StringContaining("4"));
            Assert.That(settingsManager.PackageSyncIntervalMs, Is.EqualTo(5));
            Assert.That(settingsManager.UnpackingLocation, Is.StringContaining("6"));
        }
    }
}
