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
        private SslStream _ssl;
        private TcpClient _client;
        private PacketStream _packets;

        public Action<PoopClient> OnDisconnect;

        /// <summary>
        /// The poop group this <see cref="PoopClient"/> is in.
        /// </summary>
        public string Group { get; private set; }

        /// <summary>
        /// The name that the client supplied.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The mode of this client.
        /// 0x00: DAEMON
        /// 0x01: RECEIVE
        /// 0x02: SEND
        /// </summary>
        public byte Mode { get; private set; }

        /// <summary>
        /// Create a client object using a preconfigured and connected <see cref="TcpClient"/>.
        /// </summary>
        /// <param name="client">The <see cref="TcpClient"/> to use.</param>
        public PoopClient(TcpClient client)
        {
            this._client = client;
            this._ssl = new SslStream(client.GetStream());
            this._packets = new PacketStream(_ssl);
        }

        /// <summary>
        /// Create an empty instance of <see cref="PoopClient"/>.
        /// </summary>
        public PoopClient()
        {
            _client = new TcpClient();
        }

        public async Task ConnectAsync(IPAddress host, int port)
        {
            await _client.ConnectAsync(host, port);
            _ssl = new SslStream(_client.GetStream(), false, Validate);
            _packets = new PacketStream(_ssl);
            await HandshakeAsClient();
        }

        private bool Validate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public async Task HandshakeAsClient()
        {
            try
            {
                await _ssl.AuthenticateAsClientAsync("terminat0r");
                await _packets.WriteAsync(new Packet(PacketType.AUTH, "Password123"));
                await _packets.WriteAsync(new Packet(PacketType.SETG, "XXX_PORN_GROUP_XXX"));
                await _packets.WriteAsync(new Packet(PacketType.MODE, new byte[1] { 0x00 }));
                await _packets.WriteAsync(new Packet(PacketType.NAME, $"{Environment.UserName}@{Environment.MachineName}"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<bool> HandshakeAsServer()
        {
            X509Certificate x = new X509Certificate("C:/programdata/poopchute/poopcertificate.p12", "Password123");
            await _ssl.AuthenticateAsServerAsync(x, false, System.Security.Authentication.SslProtocols.Tls12, false);

            Packet p = await _packets.ReadAsync();
            if(p.Type != PacketType.AUTH)
            {
                Console.WriteLine("packet gotted, not auth");
                await Kill();
                return false;
            }

            if(p.GetPayloadAsString() != "Password123")
            {
                Console.WriteLine("packet gotted, wronk pass: " + p.GetPayloadAsString());
                await Kill();
                return false;
            }

            p = await _packets.ReadAsync();
            if(p.Type != PacketType.SETG)
            {
                Console.WriteLine("idiot didn't set their group");
                await Kill();
                return false;
            }


            this.Group = p.GetPayloadAsString();
            if(string.IsNullOrWhiteSpace(Group))
            {
                Console.WriteLine("idiot sent invalid gruop");
                await Kill();
                return false;
            }

            p = await _packets.ReadAsync();
            if (p.Type != PacketType.MODE || p.Payload.Length != 1)
            {
                Console.WriteLine("idiot didn't set their mode");
                await Kill();
                return false;
            }

            this.Mode = p.Payload[0];
            switch (p.Payload[0])
            {
                case 0x00:
                    Console.WriteLine("mode is DAEMON");
                    break;
                case 0x01:
                    Console.WriteLine("mode is RECEIVE");
                    break;
                case 0x02:
                    Console.WriteLine("mode is SEND");
                    break;
                default:
                    Console.WriteLine("idiot set invalid mode");
                    await Kill();
                    return false;
            }

            p = await _packets.ReadAsync();
            if (p.Type != PacketType.NAME)
            {
                Console.WriteLine("idiot didn't set their name");
                await Kill();
                return false;
            }

            this.Name = p.GetPayloadAsString();
            if (string.IsNullOrWhiteSpace(Name) || Name.Length > 256)
            {
                Console.WriteLine("idiot sent invalid name");
                await _packets.WriteAsync(new Packet(PacketType.FUCK, "Bad name."));
                await Disconnect();
                return false;
            }

            return true;
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

        private async Task<bool> TryFillBufferAsync(byte[] buffer, int offset, int count)
        {
            int totalRead = 0;
            do
            {
                int r = await _ssl.ReadAsync(buffer, offset, count - totalRead);
                if (r == 0)
                    return false;
                totalRead += r;
                offset += r;

            } while (totalRead < count);

            return true;
        }
    }
}
