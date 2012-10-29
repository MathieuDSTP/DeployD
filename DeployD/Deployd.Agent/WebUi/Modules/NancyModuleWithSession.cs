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
    public abstract class NancyModuleWithSession : NancyModule
    {
        public static Func<IIocContainer> Container { get; set; }
        protected IDocumentSession DocumentSession = null;
        private ILog _log = LogManager.GetLogger(typeof (NancyModuleWithSession));
        protected IActivationBlock RequestScopeContainer = null;

        protected NancyModuleWithSession() : base()
        {
            Before += ctx =>
                {
                    _log.Debug("Opening document session");
                    RequestScopeContainer = Container().BeginBlock();
                    DocumentSession = RequestScopeContainer.Get<IDocumentSession>();
                    return null;
                };

            After += ctx =>
                {
                    _log.Debug("Closing document session");
                    if (DocumentSession != null)
                    {
                        DocumentSession.SaveChanges();
                        DocumentSession.Dispose();
                    }

                    RequestScopeContainer.Dispose();
                };
        }

        protected NancyModuleWithSession(string baseUrl) : base(baseUrl)
        {
            Before += ctx =>
            {
                _log.Debug("Opening document session");
                RequestScopeContainer = Container().BeginBlock();
                DocumentSession = RequestScopeContainer.Get<IDocumentSession>();
                return null;
            };

            After += ctx =>
            {
                _log.Debug("Closing document session");
                if (DocumentSession != null)
                {
                    DocumentSession.SaveChanges();
                    DocumentSession.Dispose();
                }

                RequestScopeContainer.Dispose();
            };
        }
    }
}