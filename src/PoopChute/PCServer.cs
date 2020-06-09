using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using PoopChuteLib;

namespace PoopChuteServer
{
    class PCServer
    {
        private static PoopServer _server;

        private static void Main(string[] args)
        {
            Console.Title = "Poop Chute Server";
            Console.WriteLine("Poop Chute Server");
            MainAsync(args).Wait();

            Console.WriteLine("Poop chute server exited");
            Console.ReadKey();
        }

        private static async Task MainAsync(string[] args)
        {
            try
            {
                _server = new PoopServer();
                await _server.RunAsync(new IPEndPoint(IPAddress.Any, 5000), 16);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
