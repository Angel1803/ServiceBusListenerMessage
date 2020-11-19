using Autofac;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBusServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using user_send_microservice.EventHandling;

namespace UserListenerServiceBus
{
    class Program
    {
        private static ServiceCollection services;
        private static ISubscriptionClient subscriptionClient;
        private static ManagementClient managementClient;
        private static SubscriptionDescription description;
        private static string topicName, subscriptionClientName, serviceBusConnectionString, serviceBusConnectionTopic;

        static void Main(string[] args)
        {
            Console.WriteLine("Recibidor Inicializado.");

            //Configuracion para leer el appsettings.json
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();

            //Valores del appsetting.json para suscripcion
            serviceBusConnectionTopic = builder.GetSection("ServiceBusConnectionTopic").Value;
            topicName = builder.GetSection("TopicName").Value;
            subscriptionClientName = builder.GetSection("SubscriptionName").Value;
            serviceBusConnectionString = builder.GetSection("ServiceBusConnectionString").Value;

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
            //Comenzamos la configuración del servicio Service Bus
            services.AddLogging().AddSingleton<IServiceBusPersisterConnection>(sp =>
            {
                //Obtiene el GetRequiredService inyectando una interfaz que tiene una clase genérica.
                var logger = sp.GetRequiredService<ILogger<DefaultServiceBusPersisterConnection>>();
                //Se obtiene la configuracion de appsettings.json para la conexion
                
                //Se inicia la conexion implementando la configuracion obtenida a la clase ServiceBusConectionStringBuilder
                var serviceBusConnection = new ServiceBusConnectionStringBuilder(serviceBusConnectionTopic);
                //Va a instanciar el DefaultServiceBusPersisterConnection con la configuracion
                return new DefaultServiceBusPersisterConnection(serviceBusConnection, logger);
            });

            ValidateSubscription().GetAwaiter().GetResult();
            RegisterEventBus(services);
        }

        static async Task ValidateSubscription()
        {
            //Se valida si existe la suscripcion al topic
            managementClient = new ManagementClient(serviceBusConnectionString);

            if (!await managementClient.SubscriptionExistsAsync(topicName, subscriptionClientName))
            {
                description = new SubscriptionDescription(topicName, subscriptionClientName)
                {
                    //Esta caracteristica se utiliza para eliminar en un tiempo determinado la suscripcion, si no se agrega,el valor por default sera "nunca".
                    //AutoDeleteOnIdle = TimeSpan.FromMinutes(5)
                    
                    //Esta caracteristica, añade el tiempo de vida de los mensajes, ya sea desde segundos, minutos, dias. En este ejemplo se le da 1 dia de vida al mensaje.
                    DefaultMessageTimeToLive = TimeSpan.FromDays(1),

                    //Esta caracteristica se establece para darle a la suscripcion cuantas entregas seran las máximas
                    MaxDeliveryCount = 20
                };

                await managementClient.CreateSubscriptionAsync(description);
                Console.WriteLine(subscriptionClientName + " Activada");
            }
            else
            {
                description = await managementClient.GetSubscriptionAsync(topicName, subscriptionClientName);
                Console.WriteLine("Esperando mensajes entrantes para " + subscriptionClientName);
            }

            subscriptionClient = new SubscriptionClient(serviceBusConnectionString, topicName, description.SubscriptionName);
        }

        //Registro del EventBus
        private static void RegisterEventBus(IServiceCollection services)
        {
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
        }

        public static void ConfigureEventBusSuscribe(IServiceCollection services)
        {
            //Llamamos al método genérico de la interfaz de IEventBus "Suscribe"
            var eventBus = services.BuildServiceProvider().GetService<IEventBus>();
            eventBus.Subscribe<ServiceBusMessage, ClienteIntegrationEventHandler>();
        }
    }
}
