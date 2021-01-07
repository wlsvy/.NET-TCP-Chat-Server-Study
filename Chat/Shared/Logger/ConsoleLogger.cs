using Shared.Interface;
using System;

namespace Shared.Logger
{
    public class ConsoleLogger : ILogger
    {
        public void Warn(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[Warning] {message}");
            Console.ResetColor();
        }

        public void Error(string caption, Exception exception)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[Error] {caption} \n - [Message] {exception.Message} \n{exception.StackTrace}");
            Console.ResetColor();
        }

        public void Info(string message)
        {
            Console.WriteLine($"[Info] {message}");
        }
    }
}
