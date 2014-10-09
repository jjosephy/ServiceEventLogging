using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceEventLogging;

namespace ServiceEventLoggerTests
{
    class TestEventLogger : ServiceEventLogger
    {
        public TestEventLogger() :
            base(new TestEventListener())
        {
        }

        public IList<string> LogLines
        {
            get
            {
                return this.LogLines;
            }
        }
    }
}
