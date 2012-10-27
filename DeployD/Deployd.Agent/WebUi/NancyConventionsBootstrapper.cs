using System;
using System.Linq;
using System.Security.Principal;
using Nancy;
using Nancy.Authentication.Basic;
using Nancy.Authentication.Forms;
using Nancy.Responses.Negotiation;
using TinyIoC;

namespace Deployd.Agent.WebUi
{
    public class NancyConventionsBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
        {
            Conventions.ViewLocationConventions.Add((viewName, model, context) => string.Concat("WebUi/Views/", viewName));

            var formsAuthConfig = new FormsAuthenticationConfiguration()
            {
                RedirectUrl = "~/login",
                RedirectQuerystringKey = "returnUrl",
                UserMapper = container.Resolve<IUserMapper>()
            };
            FormsAuthentication.Enable(pipelines, formsAuthConfig);

            var basicAuthenticationConfig = new BasicAuthenticationConfiguration(container.Resolve<IUserValidator>(), "deployd", UserPromptBehaviour.Never);
            pipelines.EnableBasicAuthentication(basicAuthenticationConfig);
        }

        protected override void RequestStartup(TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines, NancyContext context)
        {
            base.RequestStartup(container, pipelines, context);

            var credentials = ExtractCredentialsFromRequest(context);
            if(credentials !=null)
                context.CurrentUser = container.Resolve<IUserValidator>().Validate(credentials[0], credentials[1]);
        }

        private string[] ExtractCredentialsFromRequest(NancyContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].SingleOrDefault();
            if (authHeader == null)
                return null;

            var credentials = authHeader.Split(new[]{':'}, StringSplitOptions.RemoveEmptyEntries);
            if (credentials.Length != 2)
                return null;

            return credentials;
        }
    }
}
