using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceEventLogging.Events
{
    public class ServerEvent
    {
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
                this.EventLevel.ToString(),
                this.EventId,
                this.DateTime.ToString(),
                this.DebugInfo,
                this.IpAddress,
                this.Message,
                this.ServerName,
                Environment.NewLine);
        }
    }
}
