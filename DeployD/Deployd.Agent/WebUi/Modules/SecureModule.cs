using Nancy;
using Nancy.Authentication.Basic;
using Nancy.Security;

namespace Deployd.Agent.WebUi.Modules
{
    public abstract class SecureModule : NancyModuleWithScope
    {
        protected SecureModule()
        {
            this.RequiresAuthentication();
        }

        protected SecureModule(string baseUrl) : base(baseUrl)
        {
            this.RequiresAuthentication();
        }
    }
}