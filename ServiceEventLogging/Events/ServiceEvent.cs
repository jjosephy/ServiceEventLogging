
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
        /// <summary>
        /// Default Ctor for Service Event
        /// </summary>
        public ServiceEvent()
        {
            this.CorrelationId = Guid.NewGuid();
            this.DateTime = DateTime.Now;
            this.EventLevel = EventLevel.Informational;
            this.EventId = -1;
            this.IpAddress = "127.0.0.0";
        }

        /// <summary>
        /// Guid that is used as the Correlation Id to use for searaching logs and correlation to callers
        /// </summary>
        public Guid CorrelationId
        {
            get;
            set;
        }

        /// <summary>
        /// Represents the DateTime at which this Event was created
        /// </summary>
        public DateTime DateTime
        {
            get;
            set;
        }

        /// <summary>
        /// This should be set to the name of the server by default
        /// </summary>
        public string ServerName
        {
            get;
            set;
        }

        /// <summary>
        /// This should capture the Ip Address of the Server by default ?
        /// </summary>
        public string IpAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Custom message that can be added to the log.
        /// </summary>
        public string Message
        {
            get;
            set;
        }

        /// <summary>
        /// Custom Debug information that can be added to the log, such as call stack information or parameter information
        /// </summary>
        public string DebugInfo
        {
            get;
            set;
        }

        /// <summary>
        /// Event Level of the Event
        /// </summary>
        public EventLevel EventLevel
        {
            get;
            set;
        }

        /// <summary>
        /// Event Id that represents the custom event Id to be logged
        /// </summary>
        public int EventId
        {
            get;
            set;
        }

        /// <summary>
        /// Method that will turn an event into a schematized string.
        /// Schema is EventLevel | EventId | CorrelationId | DateTime | ServerName | Ip Address | Message | DebugInfo \n\r
        /// </summary>
        /// <returns>The event as a string</returns>
        public virtual string ToLogLine()
        {
            return string.Join("|",
                this.EventLevel.ToString(),
                this.EventId,
                this.CorrelationId.ToString(),
                this.DateTime.ToString(),
                string.IsNullOrWhiteSpace(this.ServerName) ? Environment.MachineName : this.ServerName, 
                this.IpAddress,
                this.Message,
                this.DebugInfo);
        }
    }
}
