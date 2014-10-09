using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceEventLogging;
using ServiceEventLoggerTests.ServiceEventExtensions;
using System.Diagnostics.Tracing;

namespace ServiceEventLoggerTests
{
    class TestEventLogger : ServiceEventLogger
    {
        static readonly TestEventListener listener = new TestEventListener();

        public TestEventLogger() :
            base(listener)
        {
        }

        public void LogMyServiceDidSomethingCoolEvent(MyCustomServiceEvent serverEvent)
        {
            serverEvent.EventId = 5000; //customize my eventId
            serverEvent.EventLevel = EventLevel.Informational;
            this.EventSource.LogServiceSuccessEvent(serverEvent.ToLogLine());
        }

        public IList<string> LogLines
        {
            get
            {
                return listener.LogLines;
            }
        }
    }
}
