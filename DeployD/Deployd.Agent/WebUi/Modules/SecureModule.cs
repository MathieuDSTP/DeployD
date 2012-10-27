using Nancy;
using Nancy.Security;

namespace Deployd.Agent.WebUi.Modules
{
    public abstract class SecureModule : NancyModule
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