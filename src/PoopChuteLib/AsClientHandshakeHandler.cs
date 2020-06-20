using System;
using System.Threading.Tasks;

namespace PoopChuteLib
{
    internal class AsClientHandshakeHandler : IHandshakeHandler
    {
        public async Task<bool> Handle(PoopClient context)
        {
            try
            {
                // TODO: implement actual shit
                await context._ssl.AuthenticateAsClientAsync("superduperpooperscooper");
                await context._packets.WriteAsync(new Packet(PacketType.AUTH, "Password123"));
                await context._packets.WriteAsync(new Packet(PacketType.SETG, "XXX_PORN_GROUP_XXX"));
                await context._packets.WriteAsync(new Packet(PacketType.MODE, new byte[1] { 0x00 }));
                await context._packets.WriteAsync(new Packet(PacketType.NAME, $"{Environment.UserName}@{Environment.MachineName}"));
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERR: CLIENT HANDSHAKE FAIL {ex.Message}");
                return false;
            }
        }
    }
}