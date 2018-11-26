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

        public SeriLogger(IOptions<SeriLoggerConfigure> options, TableBuild build, LoggerBuild loggerBuild)
        {
            TableBuild = build;
            LoggerBuild = loggerBuild;
            Options = options.Value;
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

        private Logger Build<T>()
        {
            var type = typeof(T);
            Logger logger;            

            if (!TableBuild.ContainsKey(type))
            {
                var addNewTable = TableBuild.Add(type);

                if (!addNewTable)
                    throw new TypeInitializationException(typeof(T).FullName, new Exception("Add New Table Error"));

                var isObject = typeof(Exception) == type;
                var instance = new LoggerConfiguration();

                if (Options.IsSeq)
                {
                    logger = instance?
                        .MinimumLevel.Verbose()
                        //.Enrich.WithProperty(RootProperty, null)
                        .Enrich.FromLogContext()
                        .WriteTo.Seq(Options.SeqUrl)
                        .CreateLogger();
                }
                else
                {
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

                    logger = instance?
                        .MinimumLevel.Verbose()
                        .Enrich.FromLogContext()
                        .WriteTo.MSSqlServer(connectionString: Options.ConnectString, tableName: type.Name, restrictedToMinimumLevel: LogEventLevel.Debug, formatProvider: null, autoCreateSqlTable: true, columnOptions: columnOptions)
                        .CreateLogger();
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
