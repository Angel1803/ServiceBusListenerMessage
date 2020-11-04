using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserListenerServiceBus
{
    class ServiceBusMessage : IntegrationEvent
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Content { get; set; }
    }
}
