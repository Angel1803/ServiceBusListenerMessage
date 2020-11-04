using Microsoft.Azure.ServiceBus;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UserListenerServiceBus;

namespace user_send_microservice.EventHandling
{
    class ClienteIntegrationEventHandler : IIntegrationEventHandler<ServiceBusMessage>
    {
        public Task Handle(ServiceBusMessage @event)
        {
            Console.WriteLine(@event.Content.ToString());
            return Task.CompletedTask;
        }
    }
}
