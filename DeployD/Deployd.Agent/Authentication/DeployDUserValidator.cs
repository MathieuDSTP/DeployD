using Deployd.Agent.Services.Authentication;
using Microsoft.Practices.ServiceLocation;
using Nancy.Authentication.Basic;
using Nancy.Security;

namespace Deployd.Agent.Authentication
{
    public class DeployDUserValidator  : IUserValidator
    {
        public DeployDUserValidator()
        {
        }

        public IUserIdentity Validate(string username, string password)
        {
            var authenticationService = ServiceLocator.Current.GetInstance<IAuthenticationService>();
            if (authenticationService.CredentialsAuthenticate(username, password))
            {
                return new DeploydUserIdentity(){UserName=username};
            }
            return null;
        }
    }
}