using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceEventLogging.Events
{
    internal class EventId
    {
        public static int ServiceSuccessEvent = 1000;
        public static int ServiceFailureEvent = 1001;
    }
}
