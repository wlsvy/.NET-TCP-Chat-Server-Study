using Shared.Logger;
using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading;

namespace Client
{
    public static class Program
    {
        public static void Main_Bot(CancellationTokenSource cts)
        {
            Console.WriteLine("=========================== \n   Run Client Bot! \n===========================");

            var config = LoadClientConfig();

            InitializeSingleton();

            using (var client = new Client(config))
            {
                if (client.TryConnectToServer())
                {
                    client.RunLoop(cts);
                }
            }

            DestroySingleton();

            Console.WriteLine("=========================== \n   Client Bot Terminated! \n===========================");
        }

        private static void Main(string[] args)
        {

            Console.WriteLine("=========================== \n \t Run Client! \n===========================");

            var config = LoadClientConfig();

            InitializeSingleton();

            using (var client = new Client(config))
            {
                if (client.TryConnectToServer())
                {
                    
                }
            }

            DestroySingleton();

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
