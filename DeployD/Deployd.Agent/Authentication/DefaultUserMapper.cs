using System;
using System.Linq;
using System.Text;
using Deployd.Agent.Services.Authentication;
using Microsoft.Practices.ServiceLocation;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Security;

namespace Deployd.Agent.Authentication
{
    public class DefaultUserMapper : IUserMapper
    {
        public IUserIdentity GetUserFromIdentifier(Guid identifier, NancyContext context)
        {
            var authenticationService = ServiceLocator.Current.GetInstance<IAuthenticationService>();
            return authenticationService.GetUserByAuthenticationToken(identifier);
        }
    }
}
