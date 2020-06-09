using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PoopChuteLib
{
    public class PacketStream
    {
        private const int HEADER_SIZE = 8;

        private Stream stream;

        public PacketStream(Stream stream)
        {
            this.stream = stream;
        }

        public async Task<Packet> ReadAsync()
        {
            byte[] header = new byte[HEADER_SIZE];
            if (!await TryFillBufferAsync(header, 0, HEADER_SIZE))
                return null;
            
            int length = BitConverter.ToInt32(header, 0);

            PacketType type = (PacketType)BitConverter.ToInt16(header, 4);
            
            byte[] flags = new byte[2];
            Buffer.BlockCopy(header, 6, flags, 0, 2);

            byte[] payload = new byte[length];

            if (length != 0)
            {
                if (!await TryFillBufferAsync(payload, 0, (int)length))
                    return null;
            }

            return new Packet(type, flags, payload);
        }

        private async Task<bool> TryFillBufferAsync(byte[] buffer, int offset, int count)
        {
            int totalRead = 0;
            do
            {
                int r = await stream.ReadAsync(buffer, offset, count - totalRead);
                if (r == 0)
                    return false;
                totalRead += r;
                offset += r;

            } while (totalRead < count);

            return true;
        }

        public async Task WriteAsync(Packet pack)
        {
            await stream.WriteAsync(BitConverter.GetBytes(pack.Payload.Length));
            await stream.WriteAsync(BitConverter.GetBytes((ushort)pack.Type));
            await stream.WriteAsync(pack.Flags);
            await stream.WriteAsync(pack.Payload);
            await stream.FlushAsync();
        }
    }
}
