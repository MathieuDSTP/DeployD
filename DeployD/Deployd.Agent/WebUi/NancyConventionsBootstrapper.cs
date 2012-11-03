using System;
using System.Collections.Generic;
using System.Linq;
using Deployd.Agent.Authentication;
using Deployd.Agent.Services.Authentication;
using Microsoft.Practices.ServiceLocation;
using Nancy;
using Nancy.Authentication.Basic;
using Nancy.Authentication.Forms;
using Nancy.ViewEngines.Razor;
using Ninject;
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
            var kernel = ServiceLocator.Current.GetInstance<IKernel>();
            ConfigureRequestContainer(kernel, context);
            base.RequestStartup(kernel, pipelines, context);

            using (var scope = kernel.BeginBlock())
            {
                var userValidator = scope.Get<IUserValidator>();
                var authenticationService = scope.Get<IAuthenticationService>();

                var credentials = ExtractCredentialsFromRequest(context);
                if (credentials != null)
                {
                    context.CurrentUser = userValidator.Validate(credentials[0],credentials[1]);
                }


                Guid authToken;
                if (ExtractAuthenticationTokenFromRequest(context, out authToken))
                {
                    context.CurrentUser = authenticationService.GetUserByAuthenticationToken(authToken);
                }
            }
        }

        private bool ExtractAuthenticationTokenFromRequest(NancyContext context, out Guid authToken)
        {
            authToken = Guid.Empty;
            var authHeader = context.Request.Headers["Authorization"].SingleOrDefault();

            if (authHeader != null && Guid.TryParse(authHeader, out authToken))
            {
                return true;
            }

            return false;
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

    public class DeploydRazorConfiguration : IRazorConfiguration
    {
        public IEnumerable<string> GetAssemblyNames()
        {
            return new string[]{"DeployD.Agent", "DeployD.Core"};
        }

        public IEnumerable<string> GetDefaultNamespaces()
        {
            return new string[] { "DeployD.Agent", "DeployD.Core", "DeployD.Agent.WebUi.Models" };
        }

        public bool AutoIncludeModelNamespace { get; private set; }
    }
}
