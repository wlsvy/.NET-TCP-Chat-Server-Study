using System;
using System.Text.Json;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Threading;
using Shared.Gui;
using Shared.Logger;
using Server.DataSource;
using Server.Gui;

namespace Server
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("=========================== \n \t Run Server! \n===========================");

            var config = LoadServerConfig();
            InitializeSingleton();

            using (var server = new Server(config))
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
            GlobalDataSource.I.Initialize();
        }

        private static void DestroySingleton()
        {
            GlobalDataSource.I.Destroy();
            Log.I.Destroy();
        }

        private static void RunMainThreadLoop(int timeSlicePerUpdateMSec)
        {
            var timer = new Stopwatch();
            var elapsedTimeMSec = 0L;

            timer.Start();
            ServerGuiWindow.I.Open();
            ServerGuiWindow.I.AddImguiRenderer(new ImguiDemoWindow());

            while (true)
            {
                var currentElapsedTime = timer.ElapsedMilliseconds;
                var deltaTimeMSec = currentElapsedTime - elapsedTimeMSec;
                elapsedTimeMSec = currentElapsedTime;

                if (ServerGuiWindow.I.IsWindowExist)
                {
                    ServerGuiWindow.I.Update((int)deltaTimeMSec);
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

            ServerGuiWindow.I.Destroy();
        }
    }
}
