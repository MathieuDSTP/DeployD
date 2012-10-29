using System;
using System.Linq;
using System.Text;
using Nancy.Authentication.Forms;

namespace Deployd.Agent.Services.Authentication
{
    public interface IAuthenticationService
    {
        bool CredentialsAuthenticate(string username, string password);
        Guid GenerateAuthenticationToken();
    }

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

        public Guid GenerateAuthenticationToken()
        {
            return Guid.NewGuid();
        }
    }
}
