using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceEventLoggerTests
{
    public class TestEventListener : EventListener
    {
        readonly List<string> logLines = new List<string>();
        readonly SemaphoreSlim lockContext = new SemaphoreSlim(1);

        public TestEventListener()
        {
        }

        protected async override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            await lockContext.WaitAsync();
            try
            {
                logLines.Add(eventData.Payload.First().ToString());
            }
            catch
            {
                // i dont know if this will ever actually throw but we have to makes sure to release the lock
            }
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
