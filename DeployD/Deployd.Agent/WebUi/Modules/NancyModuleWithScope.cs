using System;
using Deployd.Agent.Bootstrap;
using Deployd.Core.Hosting;
using Nancy;
using Nancy.Responses;
using Ninject.Activation.Blocks;
using log4net;

namespace Deployd.Agent.WebUi.Modules
{
    public abstract class NancyModuleWithScope : NancyModule
    {
        public static Func<IIocContainer> Container { get; set; }
        private ILog _log = LogManager.GetLogger(typeof (NancyModuleWithScope));
        protected IActivationBlock RequestScope { get; private set; }

        protected NancyModuleWithScope() : base()
        {
            var versionManager = Container().GetType<IApplicationVersionManager>();

            Before += (ctx) =>
                {
                    RequestScope = Container().BeginBlock();

                    if (versionManager.RequiresConfiguration()
                        && !Request.Path.Contains("/upgraded")
                        && !Request.Path.Contains("/login")
                        && !Request.Path.Contains("/resetpassword")
                        && !Request.Path.Contains("/passwordreset")
                        && !Request.Path.Contains("/trylogin"))
                    {
                        return new RedirectResponse("/upgraded/configuration");
                    }

                    return null;
                };

            After += (ctx) => RequestScope.Dispose();
        }

        protected NancyModuleWithScope(string baseUrl) : base(baseUrl)
        {
            Before += (ctx) =>
            {
                RequestScope = Container().BeginBlock();
                return null;
            };

            After += (ctx) => RequestScope.Dispose();
        }
    }
}