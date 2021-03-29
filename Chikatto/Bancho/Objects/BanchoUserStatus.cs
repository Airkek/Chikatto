using Chikatto.Bancho.Serialization;
using Chikatto.Constants;

namespace Chikatto.Bancho.Objects
{
    public class BanchoUserStatus : ISerializable
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
    }
}