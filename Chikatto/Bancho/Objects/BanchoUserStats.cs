using Chikatto.Bancho.Serialization;

namespace Chikatto.Bancho.Objects
{
    public struct BanchoUserStats : ISerializable
    {
        public int Id;
        public BanchoUserStatus Status;
        public long RankedScore;
        public float Accuracy;
        public int PlayCount;
        public long TotalScore;
        public int Rank;
        public short PP;
        
        public void Serialize(SerializationWriter writer)
        {
            writer.Write(Id);
            Status.Serialize(writer);
            writer.Write(RankedScore);
            writer.Write(Accuracy / 100f);
            writer.Write(PlayCount);
            writer.Write(TotalScore);
            writer.Write(Rank);
            writer.Write(PP);
        }
    }
}