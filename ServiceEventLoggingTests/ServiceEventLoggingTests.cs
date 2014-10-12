using System;
using System.Linq;
using System.Diagnostics.Tracing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceEventLogging;
using ServiceEventLogging.Events;
using ServiceEventLoggerTests.Loggers;
using ServiceEventLoggerTests.ServiceEventExtensions;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

namespace ServiceEventLoggerTests
{
    [TestClass]
    public class ServiceEventLoggingTests
    {
        readonly TestEventLogger logger = new TestEventLogger();
        readonly TestEventFileLogger fileLogger = new TestEventFileLogger();

        [TestMethod]
        public void TestAddLogLinesToFile()
        {
            fileLogger.LogServiceSuccessEvent(new ServiceEvent
            {
                DateTime = DateTime.Now,
                DebugInfo = "DebugLine",
                IpAddress = "127.0.0.0",
                Message = "Standard Server Success Event",
                ServerName = "SERVER_INFO"
            });

            fileLogger.LogServiceFailureEvent(new ServiceEvent
            {
                DateTime = DateTime.Now,
                DebugInfo = "DebugLine",
                IpAddress = "127.0.0.0",
                Message = "Standard Server Failure Event",
                ServerName = "SERVER_INFO"
            });
        }

        [TestMethod]
        public void TestAddLogLinesCustom()
        {
            var correlationId = Guid.NewGuid();
            var customDataPoint = "My Custom Data Point that is awesome";
            var myCustomEvent = CreateCustomServiceEvent(customDataPoint);
            myCustomEvent.MyCustomEventDataPoint = customDataPoint;
            myCustomEvent.CorrelationId = correlationId;
            logger.LogMyServiceDidSomethingCoolEvent(myCustomEvent);
            var line = FindLogLine(correlationId, customDataPoint);
        }

        [TestMethod]
        public void TestAddLogLinesCustom_ToFile()
        {
            var correlationId = Guid.NewGuid();
            var customDataPoint = "My Custom Data Point that is awesome";
            var myCustomEvent = CreateCustomServiceEvent(customDataPoint);

            myCustomEvent.MyCustomEventDataPoint = customDataPoint;
            myCustomEvent.CorrelationId = correlationId;
            fileLogger.LogMyServiceDidSomethingCoolEvent(myCustomEvent);
        }

        [TestMethod]
        public void TestAddLogLinesToFile_LoopEvents()
        {
            for (int i = 0; i < 10000; i++)
            {
                fileLogger.LogServiceSuccessEvent(new ServiceEvent
                {
                    DebugInfo = "DebugLine" + i,
                    Message = "Server Success Event",
                    EventId = i % 2 == 0 ? CustomTestEventId.CustomTestEventOne : CustomTestEventId.CustomTestEventTwo,
                    EventLevel = i % 2 == 0 ? EventLevel.Warning : EventLevel.Informational
                });
            }

            fileLogger.LogServiceSuccessEvent(new ServiceEvent
            {
                DebugInfo = "DebugLine_final",
                Message = "Standard Server Success Event",
                EventId = CustomTestEventId.CustomTestEventFinalizer,
                EventLevel =  EventLevel.Critical
            });
        }

        private string FindLogLine(Guid correlationId, string customDataPoint)
        {
            return this.logger.LogLines.Where(fx =>
            {
                var lines = fx.Split('|');
                return lines[2].Equals(correlationId.ToString(), StringComparison.OrdinalIgnoreCase) &&
                       lines[lines.Length - 1].Equals(customDataPoint, StringComparison.OrdinalIgnoreCase);
            }).First();
        }

        private MyCustomServiceEvent CreateCustomServiceEvent(string customDataPoint)
        {
            return new MyCustomServiceEvent
            {
                DateTime = DateTime.Now,
                DebugInfo = "DebugLine",
                IpAddress = "127.0.0.0",
                Message = "Standard Server Failure Event",
                ServerName = "SERVER_INFO",
                MyCustomEventDataPoint = customDataPoint
            };
        }
    }
}
