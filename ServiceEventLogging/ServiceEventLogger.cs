﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceEventLogging.Listener;
using System.Diagnostics.Tracing;
using ServiceEventLogging.Events;

namespace ServiceEventLogging
{
    public abstract class ServiceEventLogger
    {
        // this could be an event listener defined in the tracing library
        readonly EventListener listener;

        protected ServiceEventLogger() :
            this(new ServiceEventListener())
        {
        }

        protected ServiceEventLogger(EventListener listener)
        {
            this.listener = listener;
            Initialize();
        }

        public void LogServiceSuccessEvent(ServiceEvent serverEvent)
        {
            serverEvent.EventId = EventId.ServiceSuccessEvent;
            serverEvent.EventLevel = EventLevel.Informational;
            ServiceEventSource.EventSource.LogServiceSuccessEvent(serverEvent.ToLogLine());
        }

        public void LogServiceFailureEvent(ServiceEvent serverEvent)
        {
            serverEvent.EventId = EventId.ServiceFailureEvent;
            serverEvent.EventLevel = EventLevel.Warning;
            ServiceEventSource.EventSource.LogServiceFailureEvent(serverEvent.ToLogLine());
        }

        protected ServiceEventSource EventSource
        {
            get
            {
                return ServiceEventSource.EventSource;
            }
        }

        private void Initialize()
        {
            this.listener.EnableEvents(ServiceEventSource.EventSource, EventLevel.LogAlways);
        }
    }
}
