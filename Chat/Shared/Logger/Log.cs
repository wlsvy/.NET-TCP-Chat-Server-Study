using Shared.Util;
using System;
using System.Collections.Generic;

namespace Shared.Logger
{
    public enum LogLevel
    {
        Debug,
        Info,
        Warn,
        Error,
    }

    public class Log : Singleton<Log>
    {
        private List<ILogger> m_Loggers;

        public void Initialize()
        {
            m_Loggers = new List<ILogger>()
            {
                new ConsoleLogger(),
            };
        }

        public void Debug(string message)
        {
            foreach(var logger in m_Loggers)
            {
                logger.Log(LogLevel.Debug, message);
            }
        }

        public void Warn(string message)
        {
            foreach(var logger in m_Loggers)
            {
                logger.Log(LogLevel.Warn, message);
            }
        }

        public void Error(string caption, Exception exception)
        {
            foreach (var logger in m_Loggers)
            {
                logger.Log(LogLevel.Error, caption, exception);
            }
        }

        public void Info(string message)
        {
            foreach (var logger in m_Loggers)
            {
                logger.Log(LogLevel.Info, message);
            }
        }

    }
}
