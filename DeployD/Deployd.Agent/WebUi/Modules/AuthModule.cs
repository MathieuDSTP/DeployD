using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Deployd.Agent.Services.Authentication;
using Deployd.Core.Hosting;
using Nancy;
using Nancy.Authentication.Forms;

namespace Deployd.Agent.WebUi.Modules
{
    public class AuthModule : NancyModule
    {
        public static Func<IIocContainer> Container { get; set; }

        public AuthModule() : base()
        {
            Get["/login"] = x => View["login.cshtml", new{returnUrl=Request.Query["returnUrl"]}];

            Post["/trylogin"] = x =>
                {
                    var authenticationService = Container().GetType<IAuthenticationService>();
                if (authenticationService.CredentialsAuthenticate(Request.Form["username"], Request.Form["password"]))
                {
                    return FormsAuthentication.UserLoggedInRedirectResponse(
                        Context,
                        authenticationService.GenerateAuthenticationToken(Request.Form["username"]),
                        DateTime.Now.AddMinutes(10));
                }
                else
                {
                    return Response.AsRedirect("~/login");
                }
            };

            Get["/logout"] = x => { return FormsAuthentication.LogOutAndRedirectResponse(Context, "/"); };
        }
    }
}
