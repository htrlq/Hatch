using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LoggerUtil
{
    public static class SeriLogExtension
    {
        public static IServiceCollection AddSeriLogger(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<TableBuild>();
            serviceCollection.AddSingleton<LoggerBuild>();
            serviceCollection.AddSingleton<SeriLoggerConfiguraBuild> ();
            serviceCollection.AddScoped<ISeriLogger,SeriLogger>();

            return serviceCollection;
        }
    }

    public static class SeriLoggerContextPoolExtension
    {
        public static IServiceCollection AddSeriLoggerContextPool<TSeriLogger>(this IServiceCollection serviceCollection)
            where TSeriLogger : SeriLogger
        {
            serviceCollection.AddScoped<TSeriLogger>();

            return serviceCollection;
        }

        public static IApplicationBuilder UseSeriLogger<TSeriLogger>(this IApplicationBuilder application, Action<SeriLoggerConfigure> action)
            where TSeriLogger : SeriLogger
        {
            var configura = application.ApplicationServices.GetRequiredService<SeriLoggerConfiguraBuild>();
            var entity = new SeriLoggerConfigure();
            action.Invoke(entity);

            configura.TryAdd(typeof(TSeriLogger), entity);

            return application;
        }
    }
}