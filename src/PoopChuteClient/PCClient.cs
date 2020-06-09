using System;
using System.Net;
using System.Threading.Tasks;
using PoopChuteLib;

namespace PoopChuteClient
{
    class PCClient
    {
        public static PoopClient _client;

        private static void Main(string[] args)
        {
            Console.Title = "Poop Chute Client";
            Console.WriteLine("Poop Chute Client");
            MainAsync(args).Wait();
        }

        private static async Task MainAsync(string[] args)
        {
            _client = new PoopClient();
            await _client.ConnectAsync(IPAddress.Parse("192.168.1.19"), 5000);
            Console.ReadLine();
        }
    }
}
