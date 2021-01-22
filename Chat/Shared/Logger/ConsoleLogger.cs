using System;
using System.Threading;

namespace Shared.Logger
{
    public class ConsoleLogger : ILogger
    {
        private SpinLock m_Lock;

        public void Warn(string message)
        {
            bool lockTaken = false;
            m_Lock.Enter(ref lockTaken);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[Warning] {message}");
            Console.ResetColor();

            m_Lock.Exit();
        }

        public void Error(string caption, Exception exception)
        {
            bool lockTaken = false;
            m_Lock.Enter(ref lockTaken);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[Error] {caption} \n - [Message] {exception.Message} \n{exception.StackTrace}");
            Console.ResetColor();

            m_Lock.Exit();
        }

        public void Info(string message)
        {
            bool lockTaken = false;
            m_Lock.Enter(ref lockTaken);

            Console.WriteLine($"[Info] {message}");

            m_Lock.Exit();
        }
    }
}
