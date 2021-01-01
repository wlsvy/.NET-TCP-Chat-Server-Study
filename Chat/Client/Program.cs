using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using Client.Core;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("=========================== \n \t Run Client! \n===========================");

            var config = LoadClientConfig();

            using (var client = new Core.Client(config))
            {
            }

            Console.WriteLine("=========================== \n \t Client Terminated! \n===========================");
        }

        private static ClientConfig LoadClientConfig()
        {
            var path = Assembly.GetExecutingAssembly().Location;
            path = Directory.GetParent(path).FullName;
            path += "\\ClientConfig.json";
            var jsonString = File.ReadAllText(path);
            return JsonSerializer.Deserialize<ClientConfig>(jsonString);
        }
    }
}
