using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Deployd.Agent.Services.Authentication;
using Deployd.Core.AgentConfiguration;
using log4net;

namespace Deployd.Agent.Bootstrap
{
    public class AdminAccountBootstrap : IApplicationBootstrap
    {
        private readonly ICredentialStore _credentialStore;
        private readonly IAgentSettings _agentSettings;
        private ILog _log = LogManager.GetLogger(typeof (AdminAccountBootstrap));

        public AdminAccountBootstrap(ICredentialStore credentialStore, IAgentSettings agentSettings)
        {
            _credentialStore = credentialStore;
            _agentSettings = agentSettings;
        }

        public void OnStart()
        {
            var adminUser = _credentialStore.GetByUsername("admin");
            if (adminUser == null)
            {
                var tempPassword = Guid.NewGuid();
                _credentialStore.AddUser("admin", tempPassword.ToString());
                var resetToken = _credentialStore.CreatePasswordResetToken("admin");
                _log.Info("Created a new admin account with random password.");
                _log.InfoFormat("Browse to {0} to reset the admin password", 
                    "http://localhost:9999/resetpassword/"+resetToken);
            }
        }

        public void OnShutdown()
        {
            // nothing to do
        }
    }
}
