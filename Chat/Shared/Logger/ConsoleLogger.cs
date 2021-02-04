using System;
using System.Threading;

namespace Shared.Logger
{
    public class ConsoleLogger : ILogger
    {
        private SpinLock m_Lock;

        public void Log(LogLevel level, string message)
        {
            switch (level)
            {
                case LogLevel.Debug: Debug(message); return;
                case LogLevel.Info: Info(message); return;
                case LogLevel.Warn: Warn(message); return;
                default: return;
            }
        }

        public void Log(LogLevel level, string message, Exception exception)
        {
            if (level == LogLevel.Error)
            {
                Error(message, exception);
            }
        }

        private void Debug(string message)
        {
            bool lockTaken = false;
            m_Lock.Enter(ref lockTaken);

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"[Debug] {message}");
            Console.ResetColor();

            m_Lock.Exit();
        }

        private void Info(string message)
        {
            bool lockTaken = false;
            m_Lock.Enter(ref lockTaken);

            Console.WriteLine($"[Info] {message}");

            m_Lock.Exit();
        }

        private void Warn(string message)
        {
            bool lockTaken = false;
            m_Lock.Enter(ref lockTaken);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[Warning] {message}");
            Console.ResetColor();

            m_Lock.Exit();
        }

        private void Error(string caption, Exception exception)
        {
            bool lockTaken = false;
            m_Lock.Enter(ref lockTaken);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[Error] {caption} \n - [Message] {exception.Message} \n{exception.StackTrace}");
            Console.ResetColor();

            m_Lock.Exit();
        }
    }
}
