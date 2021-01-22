using System;
using Server.Core;
using System.Text.Json;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Threading;
using Shared.Gui;
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

                RunMainThreadLoop(config.TimeSlicePerUpdateMSec);
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

        private static void RunMainThreadLoop(int timeSlicePerUpdateMSec)
        {
            var timer = new Stopwatch();
            var veldridWindow = new VeldridWindow();
            var elapsedTimeMSec = 0L;

            timer.Start();
            veldridWindow.Open();

            while (true)
            {
                var currentElapsedTime = timer.ElapsedMilliseconds;
                var deltaTimeMSec = currentElapsedTime - elapsedTimeMSec;
                elapsedTimeMSec = currentElapsedTime;

                if (veldridWindow.IsWindowExist)
                {
                    veldridWindow.Update((int)deltaTimeMSec);
                }
                else
                {
                    break;
                }

                var updateConsumedTime = timer.ElapsedMilliseconds - elapsedTimeMSec;
                var sleepTime = timeSlicePerUpdateMSec - updateConsumedTime;
                if (sleepTime > 0)
                {
                    Thread.Sleep((int)sleepTime);
                }
            }

            veldridWindow.Dispose();
        }
    }
}
