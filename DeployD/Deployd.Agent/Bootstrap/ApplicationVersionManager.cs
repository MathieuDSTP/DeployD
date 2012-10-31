using Microsoft.Practices.ServiceLocation;
using Raven.Client;

namespace Deployd.Agent.Bootstrap
{
    public class ApplicationVersionManager : IApplicationVersionManager
    {
        private DeploydVersion _currentVersion = null;

        public ApplicationVersionManager(IDocumentSession documentSession)
        {
            _currentVersion = documentSession.Load<DeploydVersion>(1);
        }

        public bool RequiresConfiguration()
        {
            return _currentVersion == null;
        }

        public void WizardComplete()
        {
            SetVersion(_currentVersion.MigrationsVersion, false);
        }

        public int Version { 
            get
            {
                if (_currentVersion == null)
                    return 0;
                
                return _currentVersion.MigrationsVersion;
            }
        }

        public void SetVersion(int newVersion, bool configurationRequired)
        {
            var store = ServiceLocator.Current.GetInstance<IDocumentStore>();
            using (var session = store.OpenSession())
            {
                var databaseVersion = session.Load<DeploydVersion>(1);

                if (databaseVersion == null)
                {
                    databaseVersion = new DeploydVersion();
                }
                databaseVersion.MigrationsVersion = newVersion;
                databaseVersion.ConfigurationRequired = configurationRequired;
                session.SaveChanges();

                _currentVersion = databaseVersion;
            }
        }
    }
}