using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LoggerUtil
{
    public static class SeriLogExtension
    {
        public static IServiceCollection AddSeriLogger(this IServiceCollection serviceCollection,
            Action<SeriLoggerConfigure> action)
        {
            serviceCollection.Configure(action);
            serviceCollection.AddSeriLogger();

            return serviceCollection;
        }

        public static IServiceCollection AddSeriLogger(this IServiceCollection serviceCollection,
            IConfigurationSection section)
        {
            serviceCollection.Configure<SeriLoggerConfigure>(section);
            serviceCollection.AddSeriLogger();

            return serviceCollection;
        }

        public static IServiceCollection AddSeriLogger(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<TableBuild>();
            serviceCollection.AddSingleton<LoggerBuild>();
            serviceCollection.AddScoped<ISeriLogger,SeriLogger>();

            return serviceCollection;
        }
    }
}