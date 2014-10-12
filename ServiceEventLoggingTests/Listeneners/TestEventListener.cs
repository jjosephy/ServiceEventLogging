using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceEventLoggerTests.Listeners
{
    public class TestEventListener : EventListener
    {
        readonly List<string> logLines = new List<string>();
        readonly SemaphoreSlim lockContext = new SemaphoreSlim(1);

        public TestEventListener()
        {
        }

        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            lockContext.Wait();
            try
            {
                logLines.Add(eventData.Payload.First().ToString());
            }
            catch
            {}
            finally
            {
                lockContext.Release();
            }
        }

        public IList<string> LogLines
        {
            get
            {
                return this.logLines;
            }
        }
    }
}
