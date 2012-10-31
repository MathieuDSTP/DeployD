namespace Deployd.Agent.Bootstrap
{
    public interface IApplicationVersionManager
    {
        bool RequiresConfiguration();
        void WizardComplete();
        int Version { get; }
        void SetVersion(int newVersion, bool configurationRequired);
    }
}