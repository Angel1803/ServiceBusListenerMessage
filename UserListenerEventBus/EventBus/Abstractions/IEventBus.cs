using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using System;

namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions
{
    public interface IEventBus
    {

        void Publish(IntegrationEvent @event);

        /* 
         * Método genérico para súscribir donde T debe ser de tipo referencia
         * y TH deberá implementar la interfáz indicada.
         */
        void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        /*
         * Método genérico para suscripcion dinámica
         */
        void SubscribeDynamic<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;

        void UnsubscribeDynamic<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;

        //Método genérico para desuscribir.
        void Unsubscribe<T, TH>()
            where TH : IIntegrationEventHandler<T>
            where T : IntegrationEvent;
    }
}
