using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PoopChuteLib
{
    public class PoopServer
    {
        private TcpListener _listener;
        private Dictionary<string, List<PoopClient>> _clients;
        private object _clientLock = new object();

        /// <summary>
        /// Creates a Poop Chute server.
        /// </summary>
        public PoopServer()
        {
            _clients = new Dictionary<string, List<PoopClient>>();
        }

        /// <summary>
        /// Starts this Poop Chute server. Binds socket to <paramref name="listenEndPoint"/>.
        /// </summary>
        /// <param name="listenEndPoint">The <see cref="IPEndPoint"/> to listen on.</param>
        /// <returns></returns>
        public async Task RunAsync(IPEndPoint listenEndPoint, int backlog)
        {
            _listener = new TcpListener(listenEndPoint);
            _listener.Start(backlog);
            while (true) // TODO: make this stop
            {
                await ProcessClientAsync(await _listener.AcceptTcpClientAsync());
            }
        }

        private async Task ProcessClientAsync(TcpClient client)
        {
            try
            {
                PoopClient pc = new PoopClient(client);
                pc.OnDisconnect += OnPoopClientDisconnect;
                if(await pc.PerformHandshake())
                {
                    Console.WriteLine($"Welcome {pc.Name} to {pc.Group}");
                    AddClient(pc);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void OnPoopClientDisconnect(PoopClient client)
        {
            Console.WriteLine("someone disconnected");
            client.OnDisconnect -= OnPoopClientDisconnect;
            RemoveClient(client);
        }

        private void AddClient(PoopClient client)
        {
            lock(_clientLock)
            {
                if (_clients.ContainsKey(client.Group))
                {
                    _clients[client.Group].Add(client);
                }
                else
                {
                    _clients.Add(client.Group, new List<PoopClient>() { client });
                }
            }
        }

        private void RemoveClient(PoopClient client)
        {
            lock(_clientLock)
            {
                if(_clients.ContainsKey(client.Group))
                {
                    _clients[client.Group].RemoveAll(x => x.Group == client.Group);
                    if (_clients[client.Group].Count == 0)
                        _clients.Remove(client.Group);
                }
            }
        }
    }
}
