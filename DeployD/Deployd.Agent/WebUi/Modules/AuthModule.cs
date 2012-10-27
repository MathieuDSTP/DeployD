using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy;
using Nancy.Authentication.Forms;

namespace Deployd.Agent.WebUi.Modules
{
    public class AuthModule : NancyModule
    {
        
        public AuthModule()
        {
            Get["/login"] = x => View["auth/signin.cshtml"];
        }
    }
}
