using Autofac;
using Microsoft.Azure.ServiceBus;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBusServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Reflection.Metadata;
using user_send_microservice.EventHandling;

namespace UserListenerServiceBus
{
    class Program
    {
        private static ServiceCollection services;

        static void Main(string[] args)
        {
            Console.WriteLine("Recibidor Inicializado.");

            //Asignamos services al ServiceCollection
            services = new ServiceCollection();

            //Llamamos al método para configurar la conexión
            ConfigurationServiceBusConnection(services);
            ConfigureEventBusSuscribe(services);

            Console.ReadKey();
        }

        //Configuracion del ServiceBus
        public static void ConfigurationServiceBusConnection(IServiceCollection services)
        {
            //Configuracion para leer el appsettings.json
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();

            //Comenzamos la configuración del servicio Service Bus
            services.AddLogging().AddSingleton<IServiceBusPersisterConnection>(sp =>
            {
                //Obtiene el GetRequiredService inyectando una interfaz que tiene una clase genérica.
                var logger = sp.GetRequiredService<ILogger<DefaultServiceBusPersisterConnection>>();
                //Se obtiene la configuracion de appsettings.json para la conexion
                var serviceBusConnectionString = builder.GetSection("ServiceBusConnectionString").Value;
                //Se inicia la conexion implementando la configuracion obtenida a la clase ServiceBusConectionStringBuilder
                var serviceBusConnection = new ServiceBusConnectionStringBuilder(serviceBusConnectionString);
                //Va a instanciar el DefaultServiceBusPersisterConnection con la configuracion
                return new DefaultServiceBusPersisterConnection(serviceBusConnection, logger);
            });

            RegisterEventBus(services);
        }

        //Registro del EventBus
        private static void RegisterEventBus(IServiceCollection services)
        {
            //Configuracion para leer el appsettings.json
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();

            //Obtengo el nombre del cliente suscriptor desde el appsettings.json
            var subscriptionClientName = builder.GetSection("SubscriptionName").Value;

            services.AddSingleton<IEventBus, EventBusServiceBus>(sp => //sp = serviceprovider
            {
                var serviceBusPersisterConnection = sp.GetRequiredService<IServiceBusPersisterConnection>();
                var iLifetimeScope = sp.GetService<ILifetimeScope>();
                var logger = sp.GetRequiredService<ILogger<EventBusServiceBus>>();
                var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                return new EventBusServiceBus(serviceBusPersisterConnection, logger, eventBusSubcriptionsManager, subscriptionClientName, iLifetimeScope);
            });

            services.AddTransient<ClienteIntegrationEventHandler>();
            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
            //Configuracion de la suscripcion
        }

        public static void ConfigureEventBusSuscribe(IServiceCollection services)
        {
            //Llamamos al método genérico de la interfaz de IEventBus "Suscribe"
            var eventBus = services.BuildServiceProvider().GetService<IEventBus>();
            eventBus.Subscribe<ServiceBusMessage, ClienteIntegrationEventHandler>();
        }
    }
}
