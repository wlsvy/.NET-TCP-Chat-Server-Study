using System;
using Server.Core;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using System.Reflection;

namespace Server
{
    static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("=========================== \n \t Run Server! \n===========================");

            var config = LoadServerConfig();

            using (var server = new Core.Server(config))
            {
                server.Initialize();
                server.RunMainThreadLoop();
            }

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
    }
}
