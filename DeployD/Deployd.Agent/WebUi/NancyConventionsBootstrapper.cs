using Nancy;
using Nancy.Authentication.Forms;
using TinyIoC;

namespace Deployd.Agent.WebUi
{
    public class NancyConventionsBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
        {
            Conventions.ViewLocationConventions.Add((viewName, model, context) => string.Concat("WebUi/Views/", viewName));
            FormsAuthenticationConfiguration formsAuthConfig=new FormsAuthenticationConfiguration()
                {
                    RedirectUrl = "~/login",
                    RedirectQuerystringKey = "returnUrl",
                    UserMapper = container.Resolve<IUserMapper>()
                };
            FormsAuthentication.Enable(pipelines, formsAuthConfig);
        }
    }
}
