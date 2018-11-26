using System;
using LoggerUtil;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApplication1
{
    public class LoggerFilrter: IExceptionFilter
    {
        private ISeriLogger Logger { get; }

        public LoggerFilrter(ISeriLogger logger)
        {
            Logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            Logger.Error(context.Exception);
        }
    }
}