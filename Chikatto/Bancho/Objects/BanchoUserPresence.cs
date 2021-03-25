using Chikatto.Bancho.Serialization;

namespace Chikatto.Bancho.Objects
{
    public class BanchoUserPresence : ISerializable
    {
        public int Id;
        public string Name;
        /// <summary>
        /// Note: Time zone +24
        /// </summary>
        public byte Timezone;
        public byte CountryCode;
        public byte BanchoPrivileges;
        public double X;
        public double Y;
        public int Rank;
            
        public void Serialize(SerializationWriter writer)
        {
            writer.Write(Id);
            writer.Write(Name);
            writer.Write(Timezone); 
            writer.Write(CountryCode);
            writer.Write(BanchoPrivileges);
            writer.Write(X);
            writer.Write(Y);
            writer.Write(Rank);
        }

        public void Deserialize(SerializationReader reader)
        {
            Id = reader.ReadInt32();
            Name = reader.ReadString();
            Timezone = reader.ReadByte();
            CountryCode = reader.ReadByte();
            BanchoPrivileges = reader.ReadByte();
            X = reader.ReadDouble();
            Y = reader.ReadDouble();
            Rank = reader.ReadInt32();
        }
    }
}