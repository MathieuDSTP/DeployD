using System;
using System.Linq;
using System.Text;
using Deployd.Agent.Services.Authentication;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Security;

namespace Deployd.Agent.Authentication
{
    public class DefaultUserMapper : IUserMapper
    {
        private readonly IAuthenticationService _authenticationService;

        public DefaultUserMapper(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public IUserIdentity GetUserFromIdentifier(Guid identifier, NancyContext context)
        {
            return _authenticationService.GetUserByAuthenticationToken(identifier);
        }
    }
}
