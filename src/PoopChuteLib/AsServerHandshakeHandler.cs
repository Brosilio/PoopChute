using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace PoopChuteLib
{
    internal class AsServerHandshakeHandler : IHandshakeHandler
    {
        public async Task<bool> Handle(PoopClient context)
        {
            X509Certificate x = new X509Certificate("C:/programdata/poopchute/poopcertificate.p12", "Password123");
            await context._ssl.AuthenticateAsServerAsync(x, false, System.Security.Authentication.SslProtocols.Tls12, false);

            Packet p = await context._packets.ReadAsync();
            if (p.Type != PacketType.AUTH)
            {
                Console.WriteLine("packet gotted, not auth");
                await context.Kill();
                return false;
            }

            if (p.GetPayloadAsString() != "Password123")
            {
                Console.WriteLine("packet gotted, wronk pass: " + p.GetPayloadAsString());
                await context.Kill();
                return false;
            }

            p = await context._packets.ReadAsync();
            if (p.Type != PacketType.SETG)
            {
                Console.WriteLine("idiot didn't set their group");
                await context.Kill();
                return false;
            }


            context.Group = p.GetPayloadAsString();
            if (string.IsNullOrWhiteSpace(context.Group))
            {
                Console.WriteLine("idiot sent invalid gruop");
                await context.Kill();
                return false;
            }

            p = await context._packets.ReadAsync();
            if (p.Type != PacketType.MODE || p.Payload.Length != 1)
            {
                Console.WriteLine("idiot didn't set their mode");
                await context.Kill();
                return false;
            }

            context.Mode = p.Payload[0];
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
                    await context.Kill();
                    return false;
            }

            p = await context._packets.ReadAsync();
            if (p.Type != PacketType.NAME)
            {
                Console.WriteLine("idiot didn't set their name");
                await context.Kill();
                return false;
            }

            context.Name = p.GetPayloadAsString();
            if (string.IsNullOrWhiteSpace(context.Name) || context.Name.Length > 256)
            {
                Console.WriteLine("idiot sent invalid name");
                await context._packets.WriteAsync(new Packet(PacketType.FUCK, "Bad name."));
                await context.Disconnect();
                return false;
            }

            return true;
        }
    }
}