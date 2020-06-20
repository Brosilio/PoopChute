using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace PoopChuteLib
{
    public class PoopClient
    {
        internal SslStream _ssl;
        internal TcpClient _client;
        internal PacketStream _packets;
        private IHandshakeHandler _handshaker;

        public Action<PoopClient> OnDisconnect;

        /// <summary>
        /// The poop group this <see cref="PoopClient"/> is in.
        /// </summary>
        public string Group { get; internal set; }

        /// <summary>
        /// The name that the client supplied.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// The mode of this client.
        /// 0x00: DAEMON
        /// 0x01: RECEIVE
        /// 0x02: SEND
        /// </summary>
        public byte Mode { get; internal set; }

        /// <summary>
        /// Create a client object using a preconfigured and connected <see cref="TcpClient"/>.
        /// </summary>
        /// <param name="client">The <see cref="TcpClient"/> to use.</param>
        public PoopClient(TcpClient client)
        {
            this._client = client;
            this._ssl = new SslStream(client.GetStream());
            this._packets = new PacketStream(_ssl);
            this._handshaker = new AsServerHandshakeHandler();
        }

        /// <summary>
        /// Create an empty instance of <see cref="PoopClient"/>.
        /// </summary>
        public PoopClient()
        {
            this._client = new TcpClient();
            this._handshaker = new AsClientHandshakeHandler();
        }

        public async Task ConnectAsync(IPAddress host, int port)
        {
            await _client.ConnectAsync(host, port);
            _ssl = new SslStream(_client.GetStream(), false, Validate);
            _packets = new PacketStream(_ssl);
            await PerformHandshake();
        }

        private bool Validate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public Task<bool> PerformHandshake()
        {
            return _handshaker.Handle(this);
        }

        public async Task Disconnect()
        {
            await _packets.WriteAsync(new Packet(PacketType.DEAD));
            await Kill();
        }

        public async Task Kill()
        {
            await _ssl.DisposeAsync();
            _client.Dispose();
            OnDisconnect?.Invoke(this);
        }
    }
}
