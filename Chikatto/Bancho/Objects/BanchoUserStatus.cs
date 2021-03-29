using Chikatto.Bancho.Serialization;
using Chikatto.Constants;

namespace Chikatto.Bancho.Objects
{
    public class BanchoUserStatus : ISerializable, IDeserializable
    {
        public BanchoAction Action;
        public string Text;
        public string MapMd5;
        public Mods Mods;
        public GameMode Mode;
        public int MapId;
        
        public void Serialize(SerializationWriter writer)
        {
            writer.Write((byte) Action);
            writer.Write(Text);
            writer.Write(MapMd5);
            writer.Write((uint) Mods);
            writer.Write((byte) Mode);
            writer.Write(MapId);
        }


        public void Deserialize(SerializationReader reader)
        {
            Action = (BanchoAction) reader.ReadByte();
            Text = reader.ReadString();
            MapMd5 = reader.ReadString();
            Mods = (Mods) reader.ReadUInt32();
            Mode = (GameMode) reader.ReadByte();
            MapId = reader.ReadInt32();
        }
    }
}