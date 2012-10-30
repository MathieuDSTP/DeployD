using System;

namespace Deployd.Core.AgentConfiguration
{
    public interface IAgentSettingsStore
    {
        IAgentSettings LoadSettings();
        event EventHandler SettingsChanged;
        void UpdateSettings(dynamic settingsValues);
    }
}