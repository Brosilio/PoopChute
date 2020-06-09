using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PoopChuteLib
{
    public class Packet
    {
        public int Length { get; private set; }
        public PacketType Type { get; private set; }
        public byte[] Payload { get; set; }
        public byte[] Flags { get; set; }

        public Packet(PacketType type, byte[] flags, byte[] payload)
        {
            this.Type = type;
            this.Length = payload.Length;
            this.Payload = payload;
            this.Flags = flags;
        }

        public Packet(PacketType type, byte[] payload) : this(type, new byte[2], payload)
        {

        }

        public Packet(PacketType type) : this(type, new byte[2], Array.Empty<byte>())
        {

        }

        public Packet(PacketType type, string payload) : this(type, new byte[2], Encoding.ASCII.GetBytes(payload))
        {

        }

        public Packet(byte[] raw)
        {
            if (raw.LongLength < 8)
                throw new Exception("Packet missing header");

            this.Length = BitConverter.ToInt32(raw, 0);
            this.Type = (PacketType)BitConverter.ToInt16(raw, 4);

        }

        public byte[] Serialize()
        {
            using (MemoryStream ms = new MemoryStream(8 + Payload.Length))
            {
                ms.Write(BitConverter.GetBytes(Payload.Length));
                ms.Write(BitConverter.GetBytes((ushort)Type));
                ms.Write(Flags);
                if (Payload.Length > 0)
                    ms.Write(Payload);
                return ms.ToArray();
            }
        }

        public string GetPayloadAsString()
        {
            return Encoding.ASCII.GetString(Payload);
        }

        public override string ToString()
        {
            return $"len={this.Length}, rlen={this.Payload?.Length}, type={this.Type.ToString()}";
        }
    }
}
