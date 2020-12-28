using System.Collections.Generic;
using Server.Interface;
using Server.Util;

namespace Server.Logger
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

        public void Error(string message)
        {
            foreach (var logger in m_Loggers)
            {
                logger.Error(message);
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
