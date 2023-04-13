using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO.ServiceBus.Common
{
    public static class Constans
    {
        public const string ConnectionString = nameof(ConnectionString);
        public const string OrderCreatedQueueName = "OrderCreatedQueue";
        public const string OrderDeletetQueueName = "OrderDeletedQueue";
        public const string OrderTopic = "OrderTopic";
        public const string OrderCreatedSubscription = "OrderCreatedSub";
        public const string OrderDeletedSubscription = "OrderDeletedSub";
    }
}
