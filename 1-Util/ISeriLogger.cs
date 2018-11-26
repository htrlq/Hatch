using System;
using Serilog.Events;

namespace LoggerUtil
{
    public interface ISeriLogger
    {
        string RootProperty { get; }

        void Write<T>(LogEventLevel level, string messageTemplate, T propertyValue);

        void Error(Exception ex, string messageTemplate);

        void Fatal(Exception ex, string messageTemplate);

        void Debug(Exception ex, string messageTemplate);

        void Information(Exception ex, string messageTemplate);
    }
}