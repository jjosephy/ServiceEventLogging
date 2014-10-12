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

        [Event(1, Level = EventLevel.LogAlways)]
        public void LogServiceEvent(string logLine)
        {
            this.WriteEvent(1, logLine);
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
