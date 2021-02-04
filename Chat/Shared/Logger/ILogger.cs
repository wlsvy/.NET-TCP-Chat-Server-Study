using System;

namespace Shared.Logger
{
    public interface ILogger
    {
        void Log(LogLevel level, string message);
        void Log(LogLevel level, string message, Exception exception);
    }
}
