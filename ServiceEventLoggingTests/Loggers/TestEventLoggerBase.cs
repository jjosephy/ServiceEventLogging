using ServiceEventLoggerTests.ServiceEventExtensions;
using ServiceEventLogging;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceEventLoggerTests.Loggers
{
    class TestEventLoggerBase : ServiceEventLogger
    {
        protected TestEventLoggerBase(EventListener listener) :
            base(listener)
        {
        }

        protected TestEventLoggerBase() :
            base()
        { 
        }

        public void LogMyServiceDidSomethingCoolEvent(MyCustomServiceEvent serverEvent)
        {
            serverEvent.EventId = 5000; //customize my eventId
            serverEvent.EventLevel = EventLevel.Informational;
            this.EventSource.LogServiceEvent(serverEvent.ToLogLine());
        }
    }
}
