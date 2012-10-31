using System;
using System.Diagnostics;
using System.Reflection;

namespace Deployd.Agent.Bootstrap
{
    public class VersionMigrationsBootstrap : IApplicationBootstrap
    {
        private readonly IApplicationVersionManager _versionManager;

        public VersionMigrationsBootstrap(IApplicationVersionManager versionManager)
        {
            _versionManager = versionManager;
        }

        public void OnStart()
        {
            if(_versionManager.Version==0)
            {
                _versionManager.SetVersion(1, true);
            }
        }

        public void OnShutdown()
        {
        }
    }
}