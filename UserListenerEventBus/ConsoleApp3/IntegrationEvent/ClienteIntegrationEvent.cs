using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace SendMessageServiceBus.IntegrationEvent
{
    /*
     * Notas de eventos de integración:
     * Un evento es "algo que ha sucedido en el pasado", por lo tanto, su nombre debe ser
     * un evento de integración es un evento que puede causar efectos secundarios a otros microsrvicios, contextos limitados o sistemas externos.
     */
    public class ClienteIntegrationEvent
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
