using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;
using TAO.ServiceBus.Common;
using TAO.ServiceBus.Common.Events;

namespace TAO.ServiceBus.Consumer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ConsumeQueue<OrderCreatedEvent>(Constans.OrderCreatedQueueName, x=> 
            {
            Console.WriteLine($"OrderCreatedEvent ReceiveMessage with id:{x.Id} name:{x.ProductName}");
            
            }).Wait();

            ConsumeQueue<OrderDeletedEvent>(Constans.OrderDeletetQueueName, x =>
            {
                Console.WriteLine($"OrderDeletedEvent ReceiveMessage with id:{x.Id}");

            }).Wait();




            ConsumeSub<OrderCreatedEvent>(Constans.OrderTopic,Constans.OrderCreatedSubscription, x =>
            {
                Console.WriteLine($"OrderCreatedEvent ReceiveMessage with id:{x.Id} name:{x.ProductName}");

            }).Wait();

            ConsumeSub<OrderDeletedEvent>(Constans.OrderTopic, Constans.OrderDeletedSubscription, x =>
            {
                Console.WriteLine($"OrderDeletedEvent ReceiveMessage with id:{x.Id}");

            }).Wait();

            Console.ReadLine();
        }
        private static async Task ConsumeQueue<T>(string queueName, Action<T> recievedAction)
        {
            IQueueClient client = new QueueClient(Constans.ConnectionString, queueName);

            client.RegisterMessageHandler(async (message, ct) =>
            {
                var model = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(message.Body));
                recievedAction(model);

                await Task.CompletedTask;

            },
            new MessageHandlerOptions(x => Task.CompletedTask));

            Console.WriteLine($"{typeof(T).Name} is listening...");
        }

        private static async Task ConsumeSub<T>(string topicName, string subName, Action<T> recievedAction)
        {
            SubscriptionClient client = new SubscriptionClient(Constans.ConnectionString, topicName,subName);

            client.RegisterMessageHandler(async (message, ct) =>
            {
                var model = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(message.Body));
                recievedAction(model);

                await Task.CompletedTask;

            },
            new MessageHandlerOptions(x => Task.CompletedTask));

            Console.WriteLine($"{typeof(T).Name} is listening...");
        }

    }
}
