using System;

namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBus
{
    public partial class InMemoryEventBusSubscriptionsManager : IEventBusSubscriptionsManager
    {
        public class SubscriptionInfo
        {
            /*
             * Propiedades de la clase que se vuelven en lista al ser
             * utilizados en IEventBusSubscriptionsManager
             */
            public bool IsDynamic { get; }
            public Type HandlerType{ get; }

            //Constructor privado de la propia clase.
            private SubscriptionInfo(bool isDynamic, Type handlerType)
            {
                IsDynamic = isDynamic;
                HandlerType = handlerType;
            }

            public static SubscriptionInfo Dynamic(Type handlerType)
            {
                return new SubscriptionInfo(true, handlerType);
            }
            public static SubscriptionInfo Typed(Type handlerType)
            {
                return new SubscriptionInfo(false, handlerType);
            }
        }
    }
}
