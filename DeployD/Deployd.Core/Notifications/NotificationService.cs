using System;
using System.Collections.Generic;
using System.Linq;
using Deployd.Core.Hosting;
using log4net;

namespace Deployd.Core.Notifications
{
    public class NotificationService : IWindowsService, INotificationService
    {
        private readonly IEnumerable<INotifier> _notifiers;
        private ILog _log = LogManager.GetLogger(typeof (NotificationService));

        public NotificationService(IEnumerable<INotifier> notifiers)
        {
            _notifiers = notifiers.ToArray();
        }

        public void Start(string[] args)
        {
            foreach(var notifier in _notifiers)
            {
                notifier.OpenConnections();
            }
        }

        public void Stop()
        {
            foreach (var notifier in _notifiers)
            {
                if (notifier is IDisposable)
                {
                    ((IDisposable)notifier).Dispose();
                }
            }
        }

        public ApplicationContext AppContext { get; set; }
        public void NotifyAll(EventType eventType, string message)
        {
            message = System.Net.Dns.GetHostName() + " " + message;
            foreach(var notifier in _notifiers)
            {
                if (notifier.Handles(eventType))
                {
                    try
                    {
                        notifier.Notify(message);
                    } catch (Exception ex)
                    {
                        _log.Warn("Could not send notification", ex);
                    }
                }
            }
        }
    }
}
