using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using TAO.ServiceBus.Common;

namespace TAO.ServiceBus.ProducerApi.Services
{
    public class AzureService
    {
        public readonly ManagementClient _managmentClient;
        public AzureService(ManagementClient managementClient)
        {
            _managmentClient = managementClient;
        }
        public async Task SendMessageToQueue(string queueName, object messageContent, string messageType = null)
        {
            IQueueClient client = new QueueClient(Constans.ConnectionString, queueName);

            await SendMessage(client, messageContent, messageType);
        }
        public async Task CreateQueueIfNotExists(string queueName)
        {
            if (!await _managmentClient.QueueExistsAsync(queueName))
            {
                await _managmentClient.CreateQueueAsync(queueName);
            }

        }
        public async Task SendMessageToTopic(string topicName, object messageContent,string messageType = null)
        {
            TopicClient client = new TopicClient(Constans.ConnectionString, topicName);
            await SendMessage(client, messageContent);


        }
        public async Task CreateSubscriptionIfNotExists(string topicName, string subscriptionName, string messageType = null, string ruleName = null)
        {
            if (await _managmentClient.SubscriptionExistsAsync(topicName, subscriptionName))
            {
                return;
            }

            if (messageType != null)
            {
                SubscriptionDescription sd = new(topicName, subscriptionName);

                CorrelationFilter filter = new();

                filter.Properties["MessageType"] = messageType;

                RuleDescription rd = new(ruleName ?? messageType + "Rule", filter);


                await _managmentClient.CreateSubscriptionAsync(sd, rd);

            }
            else
            {
                await _managmentClient.CreateSubscriptionAsync(topicName, subscriptionName);
            }

        }
        public async Task CreateTopicIfNotExists(string topicName)
        {
            if (!await _managmentClient.TopicExistsAsync(topicName))
            {
                await _managmentClient.CreateTopicAsync(topicName);
            }

        }
        public async Task SendMessage(ISenderClient client, object messageContent, string messageType = null)
        {
            var byteArr = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageContent));

            var message = new Message(byteArr);

            message.UserProperties["MessageType"] = messageType;

            await client.SendAsync(message);
        }
    }
}
