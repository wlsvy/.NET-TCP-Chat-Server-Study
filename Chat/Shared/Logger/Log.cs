using System;
using System.Collections.Generic;
using Shared.Interface;
using Shared.Util;

namespace Shared.Logger
{
    public class Log : Singleton<Log>, ILogger
    {
        private List<ILogger> m_Loggers;

        public void Initialize()
        {
            m_Loggers = new List<ILogger>()
            {
                new ConsoleLogger(),
            };
        }

        public void Warn(string message)
        {
            foreach(var logger in m_Loggers)
            {
                logger.Warn(message);
            }
        }

        public void Error(string caption, Exception exception)
        {
            foreach (var logger in m_Loggers)
            {
                logger.Error(caption, exception);
            }
        }

        public void Info(string message)
        {
            foreach (var logger in m_Loggers)
            {
                logger.Info(message);
            }
        }

    }
}
