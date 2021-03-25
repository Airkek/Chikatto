using Chikatto.Bancho.Serialization;

namespace Chikatto.Bancho.Objects
{
    public class Presence : ISerializable
    {
        public byte Action;
        public string Text;
        public string MapMd5;
        public uint Mods;
        public byte Mode;
        public int MapId;
        
        public void Serialize(SerializationWriter writer)
        {
            writer.Write(Action);
            writer.Write(Text);
            writer.Write(MapMd5);
            writer.Write(Mods);
            writer.Write(Mode);
            writer.Write(MapId);
        }

        public void Deserialize(SerializationReader reader)
        {
            Action = reader.ReadByte();
            Text = reader.ReadString();
            MapMd5 = reader.ReadString();
            Mods = reader.ReadUInt32();
            Mode = reader.ReadByte();
            MapId = reader.ReadInt32();
        }
    }
}