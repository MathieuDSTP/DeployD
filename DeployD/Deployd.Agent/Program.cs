using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using Deployd.Agent.Bootstrap;
using Deployd.Agent.Conventions;
using Deployd.Core;
using Deployd.Core.AgentConfiguration;
using Deployd.Core.Hosting;
using Deployd.Core.Notifications;
using Microsoft.Practices.ServiceLocation;
using Ninject;
using Ninject.Modules;
using NinjectAdapter;
using Raven.Client;
using log4net;
using log4net.Appender;
using log4net.Config;
using ServiceInstaller = Deployd.Core.Hosting.ServiceInstaller;

namespace Deployd.Agent
{
    public class Program
    {
        private const string NAME = "Deployd.Agent";

        protected static ILog Logger;
        private static IKernel _kernel;
        private static ContainerWrapper _containerWrapper;

        static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            try
            {
                Logger = LogManager.GetLogger(typeof (Program));
                _kernel = new StandardKernel(new INinjectModule[] {new ContainerConfiguration()});
                
                _containerWrapper = new ContainerWrapper(_kernel);

                var locator = new NinjectServiceLocator(_kernel);
                ServiceLocator.SetLocatorProvider(()=>locator);

                IAgentSettingsManager agentSettingsManager = null;
                using (var scope = _containerWrapper.BeginBlock())
                {
                    agentSettingsManager = scope.Get<IAgentSettingsManager>();
                    agentSettingsManager.LoadSettings();
                }

                SetLogAppenderPaths(agentSettingsManager.Settings, LogManager.GetLogger("Agent.Main"));

                var notificationService = _containerWrapper.GetType<INotificationService>();

                using (var scope = _containerWrapper.BeginBlock())
                {
                    var bootstraps = scope.GetAll<IApplicationBootstrap>().ToArray();
                    foreach (var bootstrap in bootstraps)
                    {
                        bootstrap.OnStart();
                    }
                }

                new WindowsServiceRunner(args,
                                        () => _kernel.GetAll<IWindowsService>().ToArray(),
                                            installationSettings: (serviceInstaller, serviceProcessInstaller) =>
                                                                {
                                                                    serviceInstaller.ServiceName = NAME;
                                                                    serviceInstaller.StartType =
                                                                        ServiceStartMode.Manual;
                                                                    serviceProcessInstaller.Account =
                                                                        ServiceAccount.User;
                                                                },
                                        registerContainer: () => _containerWrapper,
                                        configureContext: x => { x.Log = s => Logger.Info(s); },
                                        agentSettingsManager:agentSettingsManager,
                                        notify: (x,message)=> notificationService.NotifyAll(EventType.SystemEvents, message))
                .Host();

                using (var scope = _containerWrapper.BeginBlock())
                {
                    var bootstraps = scope.GetAll<IApplicationBootstrap>().ToArray();
                    foreach (var bootstrap in bootstraps)
                    {
                        bootstrap.OnShutdown();
                    }
                }
 } catch (Exception ex)
            {
                Logger.Error("Unhandled exception", ex);
            }

            ServiceLocator.Current.GetInstance<IDocumentStore>().Dispose();
        }

        private static void SetLogAppenderPaths(IAgentSettings agentSettings, ILog log)
        {
            var appenders = log.Logger.Repository.GetAppenders().Where(a => a is FileAppender);
            foreach (FileAppender appender in appenders)
            {
                string fileName = Path.GetFileName(appender.File);
                appender.File = Path.Combine(agentSettings.LogsDirectory.MapVirtualPath(), fileName);
                appender.ActivateOptions();
            }
        }
    }

    [RunInstaller(true)]
    public class Installer : ServiceInstaller
    {
    }
}
