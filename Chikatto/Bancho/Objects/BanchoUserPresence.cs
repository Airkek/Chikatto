using System;
using Chikatto.Bancho.Serialization;
using Chikatto.Constants;

namespace Chikatto.Bancho.Objects
{
    public class BanchoUserPresence : ISerializable
    {
        public int Id;
        public string Name;
        public byte Timezone; // Note: Time zone +24
        public byte CountryCode; //TODO: CountryCode dictionary
        public BanchoPermissions BanchoPermissions;
        public double Longitude;
        public double Latitude;
        public int Rank;
            
        public void Serialize(SerializationWriter writer)
        {
            writer.Write(Id);
            writer.Write(Name);
            writer.Write(Timezone); 
            writer.Write(CountryCode);
            writer.Write((byte)BanchoPermissions);
            writer.Write(Longitude);
            writer.Write(Latitude);
            writer.Write(Rank);
        }
    }
}