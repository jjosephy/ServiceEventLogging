
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceEventLogging.Events;

namespace ServiceEventLoggerTests.ServiceEventExtensions
{
    class MyCustomServiceEvent : ServiceEvent
    {
        public MyCustomServiceEvent()
        {
            this.MyCustomEventDataPoint = "I love extension points";
        }

        public string MyCustomEventDataPoint
        {
            get;
            set;
        }

        public override string ToLogLine()
        {
            return string.Join("|", base.ToLogLine(), this.MyCustomEventDataPoint);
        }
    }
}
