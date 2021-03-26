using Chikatto.Bancho.Serialization;
using Chikatto.Constants;

namespace Chikatto.Bancho.Objects
{
    public class BanchoUserStats : ISerializable
    {
        public int Id;
        public BanchoAction Action;
        public string Text;
        public string MapMd5;
        public Mods Mods;
        public GameMode Mode;
        public int MapId;
        public long RankedScore;
        public float Accuracy;
        public int PlayCount;
        public long TotalScore;
        public int Rank;
        public short PP;
        
        public void Serialize(SerializationWriter writer)
        {
            writer.Write(Id);
            writer.Write(Text);
            writer.Write((byte)Action);
            writer.Write(MapMd5);
            writer.Write((uint) Mods);
            writer.Write((byte) Mode);
            writer.Write(MapId);
            writer.Write(RankedScore);
            writer.Write(Accuracy);
            writer.Write(PlayCount);
            writer.Write(TotalScore);
            writer.Write(Rank);
            writer.Write(PP);
        }
    }
}