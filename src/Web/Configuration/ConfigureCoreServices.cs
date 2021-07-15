﻿using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Services;
using Microsoft.eShopWeb.Infrastructure.Data;
using Microsoft.eShopWeb.Infrastructure.Logging;
using Microsoft.eShopWeb.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.eShopWeb.Web.Configuration
{
    public static class ConfigureCoreServices
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped(typeof(IAsyncRepository<>), typeof(EfRepository<>));
            services.AddScoped<IBasketService, BasketService>();
<<<<<<< Updated upstream
=======
            services.AddScoped<IOrderItemsReserver, OrderItemsReserver>(x=> new OrderItemsReserver(configuration.GetValue<string>("OrderItemsReserverServiceUrl")));
            services.AddScoped<IOrderProcessor, OrderProcessor>(x=> new OrderProcessor(configuration.GetValue<string>("OrderProcessorServiceUrl")));
            services.AddScoped<IServiceBusService, ServiceBusService>(x=> new ServiceBusService(configuration.GetConnectionString("ServiceBusConnection"), configuration.GetValue<string>("ServiceBusServiceQueueName")));
>>>>>>> Stashed changes
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddSingleton<IUriComposer>(new UriComposer(configuration.Get<CatalogSettings>()));
            services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));
            services.AddTransient<IEmailSender, EmailSender>();

            return services;
        }
    }
}
