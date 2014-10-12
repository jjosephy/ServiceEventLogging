
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceEventLogging.Listener;
using System.Diagnostics.Tracing;
using ServiceEventLogging.Events;

namespace ServiceEventLogging
{
    public abstract class ServiceEventLogger
    {
        readonly EventListener listener;

        protected ServiceEventLogger() :
            this(new ServiceEventListener())
        {
        }

        protected ServiceEventLogger(EventListener listener)
        {
            this.listener = listener;
            Initialize();
        }

        public void LogServiceSuccessEvent(ServiceEvent serviceEvent)
        {
            serviceEvent.EventId = serviceEvent.EventId == -1 ? EventId.ServiceSuccessEvent : serviceEvent.EventId;
            ServiceEventSource.EventSource.LogServiceEvent(serviceEvent.ToLogLine());
        }

        public void LogServiceFailureEvent(ServiceEvent serviceEvent)
        {
            serviceEvent.EventId = serviceEvent.EventId == -1 ? EventId.ServiceFailureEvent : serviceEvent.EventId;
            ServiceEventSource.EventSource.LogServiceEvent(serviceEvent.ToLogLine());
        }

        protected ServiceEventSource EventSource
        {
            get
            {
                return ServiceEventSource.EventSource;
            }
        }

        private void Initialize()
        {
            this.listener.EnableEvents(ServiceEventSource.EventSource, EventLevel.LogAlways);
        }
    }
}
