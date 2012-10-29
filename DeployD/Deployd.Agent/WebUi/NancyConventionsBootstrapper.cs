using System;
using System.Linq;
using System.Security.Principal;
using Deployd.Agent.Conventions;
using Microsoft.Practices.ServiceLocation;
using Nancy;
using Nancy.Authentication.Basic;
using Nancy.Authentication.Forms;
using Nancy.Responses.Negotiation;
using Ninject;
using Raven.Client;
using TinyIoC;
using log4net;

namespace Deployd.Agent.WebUi
{
    public class NancyConventionsBootstrapper : Nancy.Bootstrappers.Ninject.NinjectNancyBootstrapper
    {
        private ILog log = LogManager.GetLogger(typeof (NancyConventionsBootstrapper));

        protected override void ApplicationStartup(Ninject.IKernel container, Nancy.Bootstrapper.IPipelines pipelines)
        {
            ConfigureApplicationContainer(ServiceLocator.Current.GetInstance<IKernel>());
            Conventions.ViewLocationConventions.Add((viewName, model, context) => string.Concat("WebUi/Views/", viewName));

            var formsAuthConfig = new FormsAuthenticationConfiguration()
            {
                RedirectUrl = "~/login",
                RedirectQuerystringKey = "returnUrl",
                UserMapper = ServiceLocator.Current.GetInstance<IUserMapper>()
            };
            FormsAuthentication.Enable(pipelines, formsAuthConfig);

            var basicAuthenticationConfig = new BasicAuthenticationConfiguration(ServiceLocator.Current.GetInstance<IUserValidator>(), "deployd", UserPromptBehaviour.Never);
            pipelines.EnableBasicAuthentication(basicAuthenticationConfig);
        }

        protected override void RequestStartup(Ninject.IKernel container, Nancy.Bootstrapper.IPipelines pipelines, NancyContext context)
        {
            base.RequestStartup(container, pipelines, context);

            var credentials = ExtractCredentialsFromRequest(context);
            if(credentials !=null)
                context.CurrentUser = container.Get<IUserValidator>().Validate(credentials[0], credentials[1]);
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
