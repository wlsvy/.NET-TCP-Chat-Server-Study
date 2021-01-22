using System;

namespace Shared.Logger
{
    public interface ILogger
    {
        void Info(string message);
        void Warn(string message);
        void Error(string caption, Exception exception);
    }
}
