using System;
using IMSSampleApplication.App_Start;

namespace IMSConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseAddress = "https://localhost:44300";
            if (args.Length > 0) baseAddress = args[0];

            var server = SelfHostStarter.Start(baseAddress);
            Console.WriteLine($"Server started at {baseAddress}. Press Enter to stop.");
            Console.ReadLine();

            server.CloseAsync().Wait();
            server.Dispose();
        }
    }
}
