using System.Collections.Generic;
using Nancy.Security;

namespace Deployd.Agent.Authentication
{
    public class DeploydUserIdentity : IUserIdentity
    {
        public string UserName { get; set; }
        public IEnumerable<string> Claims { get; set; }
    }
}