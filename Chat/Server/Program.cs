using System;
using Server.Core;
using System.Text.Json;
using System.IO;
using System.Reflection;
using Shared.Logger;

namespace Server
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("=========================== \n \t Run Server! \n===========================");

            var config = LoadServerConfig();
            InitializeSingleton();

            using (var server = new Core.Server(config))
            {
                server.Start();
                server.RunMainThreadLoop();
            }

            DestroySingleton();

            Console.WriteLine("=========================== \n \t Server Terminated! \n===========================");
        }

        private static ServerConfig LoadServerConfig()
        {
            var path = Assembly.GetExecutingAssembly().Location;
            path = Directory.GetParent(path).FullName;
            path += "\\ServerConfig.json";

            var jsonString = File.ReadAllText(path);
            return JsonSerializer.Deserialize<ServerConfig>(jsonString);
        }

        private static void InitializeSingleton()
        {
            Log.I.Initialize();
        }

        private static void DestroySingleton()
        {
            Log.I.Destroy();
        }
    }
}
