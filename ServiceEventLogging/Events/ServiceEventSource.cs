using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceEventLogging.Events
{
    public sealed class ServiceEventSource : EventSource
    {
        private static ServiceEventSource source = new ServiceEventSource();

        public ServiceEventSource()
        {
        }

        [Event(1000, Level = EventLevel.Informational)]
        public void LogServiceSuccessEvent(string logLine)
        {
            this.WriteEvent(EventId.ServiceSuccessEvent, logLine);
        }

        [Event(1001, Level = EventLevel.Informational)]
        public void LogServiceFailureEvent(string logLine)
        {
            this.WriteEvent(EventId.ServiceFailureEvent, logLine);
        }

        public static ServiceEventSource EventSource
        {
            get
            {
                return source;
            }
        }
    }
}
