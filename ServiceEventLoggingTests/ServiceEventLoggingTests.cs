using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceEventLogging;
using ServiceEventLogging.Events;

namespace ServiceEventLoggerTests
{
    [TestClass]
    public class ServiceEventLoggingTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var logger = new TestEventLogger();

            logger.LogServerSuccessEvent(new ServerEvent
            {
                DateTime = DateTime.Now,
                DebugInfo = "DebugLine",
                IpAddress = "127.0.0.0",
                Message = "Standard Server Event",
                ServerName = "SERVER_INFO"
            });

            logger.LogServerFailureEvent(new ServerEvent
            {
                DateTime = DateTime.Now,
                DebugInfo = "DebugLine",
                IpAddress = "127.0.0.0",
                Message = "Standard Server Event",
                ServerName = "SERVER_INFO"
            });
        }
    }
}
