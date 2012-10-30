using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Deployd.Agent.Services.Authentication;
using Deployd.Core.Hosting;
using Microsoft.Practices.ServiceLocation;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Helpers;
using Nancy.Responses;
using Ninject;
using Raven.Client;
using Raven.Client.Linq;

namespace Deployd.Agent.WebUi.Modules
{
    public class AuthModule : NancyModuleWithScope
    {
        public AuthModule() : base()
        {
            Get["/login"] = x => View["login.cshtml", new{returnUrl=Request.Query["returnUrl"]}];

            Post["/trylogin"] = x =>
                {

                    var authenticationService = RequestScope.Get<IAuthenticationService>();
                        if (authenticationService.CredentialsAuthenticate(Request.Form["username"],
                                                                          Request.Form["password"]))
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

            Get["/logout"] = x => FormsAuthentication.LogOutAndRedirectResponse(Context, "/");

            Get["/resetpassword/{token}"] = x =>
                {
                        try
                        {
                            new Guid(HttpUtility.UrlDecode(x.token));
                        }
                        catch
                        {
                            return Response.AsRedirect("~/");
                        }

                        var credentialStore = RequestScope.Get<ICredentialStore>();
                        if (!credentialStore.VerifyPasswordResetToken(x.token))
                        {
                            return Response.AsRedirect("~/");
                        }

                        return View["resetpassword.cshtml", new PasswordResetViewModel() { PasswordResetToken = x.token }];
                };
            Post["/resetpassword/{token}"] = x =>
                {
                    var credentialStore = RequestScope.Get<ICredentialStore>();
                    credentialStore.ResetPassword(x.token, Request.Form["password"]);
                    return Response.AsRedirect("/passwordreset");
                };
            Get["/passwordreset"] = x => View["passwordreset.cshtml"];
        }
    }

    public class PasswordResetViewModel
    {
        public string PasswordResetToken { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
