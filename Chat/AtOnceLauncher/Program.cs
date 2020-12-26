using System;
using System.IO;
using System.Diagnostics;

namespace AtOnceLauncher
{
    class Program
    {
        public const string ROOT_DIRECTORY_NAME = ".NET-TCP-Chat-Server-Study";

        static void Main(string[] args)
        {
            Console.WriteLine(
                @"===============================
At Once Launcher
===============================");

            var directory = Directory.GetParent(Directory.GetCurrentDirectory());
            while (directory != null &&
                directory.Name != ROOT_DIRECTORY_NAME)
            {
                directory = directory.Parent;
            }

            if(directory.Name != ROOT_DIRECTORY_NAME)
            {
                Console.WriteLine("프로젝트 루트 폴더를 찾지 못했습니다. 프로그램을 종료합니다.");
                return;
            }

            var serverProjectPath = Path.Combine(directory.FullName, "Chat", "Server");
            var clientProjectPath = Path.Combine(directory.FullName, "Chat", "Client");

            var serverExecutable = Directory.GetFiles(serverProjectPath, "Server.exe", SearchOption.AllDirectories);
            var clientExecutable = Directory.GetFiles(clientProjectPath, "Client.exe", SearchOption.AllDirectories);

            var serverProcess = Process.Start(serverExecutable[0]);
        }
    }
}
