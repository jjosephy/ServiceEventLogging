using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceEventLogging;
using ServiceEventLogging.Events;
using ServiceEventLoggerTests.Loggers;
using ServiceEventLoggerTests.ServiceEventExtensions;

namespace ServiceEventLoggerTests
{
    [TestClass]
    public class ServiceEventLoggingTests
    {
        readonly TestEventLogger logger = new TestEventLogger();

        [TestMethod]
        public void TestAddLogLinesToFile()
        {
            var fileLogger = new TestEventFileLogger();

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

        private string FindLogLine(Guid correlationId, string customDataPoint)
        {
            return this.logger.LogLines.Where(fx =>
            {
                var lines = fx.Split('|');
                return lines[0].Equals(correlationId.ToString(), StringComparison.OrdinalIgnoreCase) &&
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
