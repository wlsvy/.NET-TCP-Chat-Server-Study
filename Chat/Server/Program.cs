using System;

namespace Server
{
    class Program
    {
        public const int TIME_SLICE_PER_UPDATE_MSEC = 30;
        static void Main(string[] args)
        {
            Console.WriteLine("=========================== \n \t Run Server! \n===========================");

            using (var server = new Core.Server())
            {
                server.Initialize();
                server.RunMainThreadLoop(TIME_SLICE_PER_UPDATE_MSEC);
            }

            Console.WriteLine("=========================== \n \t Server Terminated! \n===========================");
        }
    }
}
