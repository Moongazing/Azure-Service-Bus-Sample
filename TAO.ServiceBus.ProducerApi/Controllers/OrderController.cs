using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TAO.ServiceBus.Common;
using TAO.ServiceBus.Common.Dto;
using TAO.ServiceBus.Common.Events;
using TAO.ServiceBus.ProducerApi.Services;

namespace TAO.ServiceBus.ProducerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AzureService _azureService;
        public OrderController(AzureService azureService)
        {
            _azureService = azureService;
        }
        [HttpPost]
        public async Task CreateOrderQueue(OrderDto orderDto)
        {
            var orderCreatedEvent = new OrderCreatedEvent()
            {
                Id = orderDto.Id,
                ProductName = orderDto.Name,
                CreatedOn = DateTime.Now
            };
            await _azureService.CreateQueueIfNotExists(Constans.OrderCreatedQueueName);
            await _azureService.SendMessageToQueue(Constans.OrderCreatedQueueName, orderCreatedEvent);
        }
        [HttpDelete("{id}")]
        public async Task DeleteQueue(int id)
        {
            var orderDeletedEvent = new OrderDeletedEvent()
            {
                Id = id,
                CreatedOn = DateTime.Now
            };
            await _azureService.CreateQueueIfNotExists(Constans.OrderDeletetQueueName);
            await _azureService.SendMessageToQueue(Constans.OrderCreatedQueueName, orderDeletedEvent);
        }
        [HttpPost("CreatedOrderTopic")]
        public async Task CreateOrderTopic(OrderDto orderDto)
        {
            var orderCreatedEvent = new OrderCreatedEvent()
            {
                Id = orderDto.Id,
                ProductName = orderDto.Name,
                CreatedOn = DateTime.Now
            };

            await _azureService.CreateTopicIfNotExists(Constans.OrderTopic);

            await _azureService.CreateSubscriptionIfNotExists(Constans.OrderTopic, Constans.OrderCreatedSubscription, "OrderCreated", "OrderCreatedOnly");

            await _azureService.SendMessageToTopic(Constans.OrderTopic, orderCreatedEvent, "OrderCreated");
        }
        [HttpDelete("{topicId}")]
        public async Task DeleteTopic(int id)
        {
            var orderDeletedEvent = new OrderDeletedEvent()
            {
                Id = id,
                CreatedOn = DateTime.Now
            };
            await _azureService.CreateTopicIfNotExists(Constans.OrderDeletetQueueName);
            await _azureService.CreateSubscriptionIfNotExists(Constans.OrderTopic, Constans.OrderDeletedSubscription,"OrderDeleted", "OrderDeletedOnly");
            await _azureService.SendMessageToTopic(Constans.OrderCreatedQueueName, orderDeletedEvent, "OrderDeleted");
        }
    }
}
