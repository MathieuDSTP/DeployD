using System;
using System.Linq;
using System.Text;
using Deployd.Agent.Authentication;
using Nancy.Authentication.Forms;

namespace Deployd.Agent.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ICredentialStore _credentialStore;

        public AuthenticationService(ICredentialStore credentialStore)
        {
            _credentialStore = credentialStore;
        }

        public bool CredentialsAuthenticate(string username, string password)
        {
            return _credentialStore.ValidateCredentials(username, password);
        }

        public Guid GenerateAuthenticationToken(string username)
        {
            Guid token = Guid.NewGuid();
            _credentialStore.SetAccessToken(username, token);
            return token;
        }

        public DeploydUserIdentity GetUserByAuthenticationToken(Guid guid)
        {
            var user = _credentialStore.GetByAccessToken(guid);
            if(user !=null)
                return new DeploydUserIdentity()
                    {
                        UserName=user.Id
                    };

            return null;
        }
    }
}
