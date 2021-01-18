using System;
using System.IO;
using System.Diagnostics;
using System.Text.Json;
using Server.Core;

namespace Launcher
{
    class Program
    {
        public const string ROOT_DIRECTORY_NAME = ".NET-TCP-Chat-Server-Study";

        static void Main(string[] args)
        {
            Console.WriteLine(
                @"===============================
Launcher Start
===============================");

            var directory = FindRootDirectory();
            if(directory == null)
            {
                Console.WriteLine($"{nameof(Launcher)}.{nameof(Main)}  프로젝트 루트 폴더를 찾지 못했습니다. 프로그램을 종료합니다.");
                return;
            }

            var serverProjectPath = Path.Combine(directory.FullName, "Chat", "Server");
            var clientProjectPath = Path.Combine(directory.FullName, "Chat", "Client");

            var serverExecutable = Directory.GetFiles(serverProjectPath, "Server.exe", SearchOption.AllDirectories);
            var clientExecutable = Directory.GetFiles(clientProjectPath, "Client.exe", SearchOption.AllDirectories);

            var serverProcess = Process.Start(serverExecutable[0]);

            Console.WriteLine(
    @"===============================
Launched All Executables!!!
===============================");

            serverProcess.WaitForExit();

            Console.WriteLine(
@"===============================
Launcher Terminated
===============================");
        }

        private static DirectoryInfo FindRootDirectory()
        {
            var directory = Directory.GetParent(Directory.GetCurrentDirectory());
            while (directory != null &&
                directory.Name != ROOT_DIRECTORY_NAME)
            {
                directory = directory.Parent;
            }

            if (directory == null ||
                directory.Name != ROOT_DIRECTORY_NAME)
            {
                return null;
            }

            return directory;
        }
    }
}
