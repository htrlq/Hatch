using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Context;
using Serilog.Core.Enrichers;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System;
using System.Collections.Generic;
using System.Data;
using Serilog.Core;

namespace LoggerUtil
{

    public class SeriLogger: ISeriLogger
    {
        private LoggerBuild LoggerBuild { get; }
        private TableBuild TableBuild { get; }
        private SeriLoggerConfigure Options { get; }

        public SeriLogger(SeriLoggerConfiguraBuild configuraBuild, TableBuild build, LoggerBuild loggerBuild)
        {
            TableBuild = build;
            LoggerBuild = loggerBuild;
            var type = GetType();
            Options = configuraBuild[type];
        }

        public virtual string RootProperty => "SourceProperty";

        public void Write<T>(LogEventLevel level, string messageTemplate, T propertyValue)
        {
            var logger = Build<T>();
            var type = typeof(T);
            var list = TableBuild[type];

            var entity = list.ConvertAll(item =>
            {
                var propertyInfo = type.GetProperty(item.Property);
                var value = propertyInfo?.GetValue(propertyValue);
                return new PropertyEnricher(item.Property, value);
            });

            using (LogContext.Push(entity.ToArray()))
                logger?.Write(level, messageTemplate, propertyValue);
        }

        private Logger CreateSeq(string url, string apiKey = "")
        {
            var instance = new LoggerConfiguration();

            return instance?
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .WriteTo.Seq(url, apiKey: apiKey)
                .CreateLogger();
        }

        private Logger CreateSqlServer(Type type, string connectString)
        {
            var instance = new LoggerConfiguration();
            var isObject = typeof(Exception) == type;

            ColumnOptions columnOptions = null;
            if (!isObject)
            {
                columnOptions = new ColumnOptions()
                {
                    AdditionalDataColumns = TableBuild[type].ConvertAll(item => new DataColumn()
                        { DataType = item.DataType, ColumnName = item.Property })
                };

                columnOptions.Store.Add(StandardColumn.LogEvent);
            }

            return instance?
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .WriteTo.MSSqlServer(connectionString: connectString, tableName: type.Name, restrictedToMinimumLevel: LogEventLevel.Debug, formatProvider: null, autoCreateSqlTable: true, columnOptions: columnOptions)
                .CreateLogger();
        }

        private Logger Build<T>()
        {
            var type = typeof(T);
            Logger logger;            

            if (!TableBuild.ContainsKey(type))
            {
                var addNewTable = TableBuild.Add(type);

                if (!addNewTable)
                    throw new TypeInitializationException(typeof(T).FullName, new Exception("Add New Table Error"));

                if (Options.IsSeq)
                {
                    var seq = Options.Seq;
                    logger = CreateSeq(seq.Url, seq.ApiKey);
                }
                else
                {
                    logger = CreateSqlServer(type, Options.ConnectString);
                }

                var addNewLogger = LoggerBuild.Add(type, logger);

                if (!addNewLogger)
                    throw new TypeInitializationException(typeof(T).FullName, new Exception("Add New Logger Error"));
            }
            else
            {
                logger = LoggerBuild[type];
            }

            return logger;
        }

        public void Error(Exception ex, string messageTemplate = "")
        {
            var logger = Build<Exception>();
            logger.Error(ex, messageTemplate);
        }

        public void Fatal(Exception ex, string messageTemplate = "")
        {
            var logger = Build<Exception>();
            logger.Fatal(ex, messageTemplate);
        }

        public void Debug(Exception ex, string messageTemplate = "")
        {
            var logger = Build<Exception>();
            logger.Debug(ex, messageTemplate);
        }

        public void Information(Exception ex, string messageTemplate = "")
        {
            var logger = Build<Exception>();
            logger.Information(ex, messageTemplate);
        }
    }
}
