using System;
using System.IO;
using Chikatto.Bancho.Enums;

namespace Chikatto.Bancho
{
    public class Packet 
    {
        public readonly PacketType Type;
        public byte[] Data;

        public Packet(PacketType type)
        {
            Type = type;
            Data = Array.Empty<byte>();
        }
        
        public Packet(PacketType type, byte[] data)
        {
            Type = type;
            Data = data;
        }

        public void WriteToStream(BinaryWriter writer)
        {
            writer.Write((ushort) Type);
            writer.Write((byte) 0); // pad byte
            writer.Write(Data.Length);
            if(Data.Length != 0)
                writer.Write(Data);
        }

        public static Packet FromStream(BinaryReader reader)
        {
            var type = (PacketType) reader.ReadUInt16();
            reader.ReadByte(); // pad byte
            var dataLength = reader.ReadInt32();
            var data = dataLength != 0 ? reader.ReadBytes(dataLength) : Array.Empty<byte>();
            
            return new Packet(type, data);
        }
        
        public override string ToString() => $"<{Type} ({Data.Length})>";
    }
}