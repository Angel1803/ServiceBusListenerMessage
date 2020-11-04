using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using static Microsoft.eShopOnContainers.BuildingBlocks.EventBus.InMemoryEventBusSubscriptionsManager;

namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBus
{
    public interface IEventBusSubscriptionsManager
    {
        bool IsEmpty { get; }
        //Representa el método que manejará el evento que provee datos.
        event EventHandler<string> OnEventRemoved;

        /*
         * Los métodos genéricos son para generalizar sus
         * funcionalidades y permitir utilizarlos con varios
         * tipos de datos, en tu defecto, TH es el nombre del 
         * parámetro genérico, esta encerrado en "<" y ">" el cual
         * representa que AddDynamicSubscription es genérico y operará
         * sobre un tipo cualquiera al que llamaremos TH.
         * 
         * Cuando encontramos where T:ICualquierInterface se trata de una 
         * restricción mediante la cual 
         * estamos indicando al compilador que el tipo T 
         * podrá ser cualquiera, siempre que implementente 
         * el interfaz ICualquierInterface lo que permitirá 
         * realizar la comparación.
         */

        //Método genérico para agregar una suscripción dinámica.
        void AddDynamicSubscription<TH>(string eventName)
           where TH : IDynamicIntegrationEventHandler;

        //Método genérico para agregar una suscripción
        void AddSubscription<T, TH>()
           where T : IntegrationEvent
           where TH : IIntegrationEventHandler<T>;

        //Método genérico para remover la suscripción
        void RemoveSubscription<T, TH>()
             where TH : IIntegrationEventHandler<T>
             where T : IntegrationEvent;

        //Método genérico para remover la suscripción dinámica
        void RemoveDynamicSubscription<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;

        //Método genérico con restricción de la implementacion IIntegrationEvent.
        bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent;

        //Método genérico sin restricción para ver si tiene suscripcion al evento.
        bool HasSubscriptionsForEvent(string eventName);

        //Representa un tipo de declaración tipo parámetro (preguntar)
        Type GetEventTypeByName(string eventName);
        
        void Clear();

        //Método Interfaz genérica que crea una lista de la clase SuscriptionInfo
        //Tiene una restriccion de tipo referencia IntegrationEvent
        IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent;

        //Método con Interfáz no genérica que crea una lista de la clase SuscriptionInfo
        IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);
        string GetEventKey<T>();
    }
}