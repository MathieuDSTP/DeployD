namespace Deployd.Agent.Bootstrap
{
    public interface IApplicationBootstrap
    {
        void OnStart();
        void OnShutdown();
    }
}