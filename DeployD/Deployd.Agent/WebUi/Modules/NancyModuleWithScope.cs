using System;
using Deployd.Core.Hosting;
using Nancy;
using Nancy.Session;
using Ninject;
using Ninject.Activation.Blocks;
using Raven.Client;
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
            Before += (ctx) =>
                {
                    RequestScope = Container().BeginBlock();
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