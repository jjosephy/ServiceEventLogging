using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceEventLogging.Events
{
    /// <summary>
    /// Events can allow us to build a "schema" around logs
    /// </summary>
    public class ServiceEvent
    {
        public ServiceEvent()
        {
            this.CorrelationId = Guid.NewGuid();
        }

        public Guid CorrelationId
        {
            get;
            set;
        }

        public DateTime DateTime
        {
            get;
            set;
        }

        public string ServerName
        {
            get;
            set;
        }

        public string IpAddress
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }

        public string DebugInfo
        {
            get;
            set;
        }

        public EventLevel EventLevel
        {
            get;
            set;
        }

        public int EventId
        {
            get;
            set;
        }

        public virtual string ToLogLine()
        {
            return string.Join("|",
                this.CorrelationId.ToString(),
                this.EventLevel.ToString(),
                this.EventId,
                this.DateTime.ToString(),
                this.DebugInfo,
                this.IpAddress,
                this.Message,
                string.Format("{0}{1}", this.ServerName,Environment.NewLine));
        }
    }
}
