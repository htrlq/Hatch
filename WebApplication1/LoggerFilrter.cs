using System;
using LoggerUtil;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace WebApplication1
{
    public class LoggerAttribute: ActionFilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService(typeof(ISeriLogger)) as ISeriLogger;
            logger.Error(context.Exception.InnerException);
        }
    }
}