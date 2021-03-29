﻿using System;
using Chikatto.Bancho.Serialization;
using Chikatto.Constants;

namespace Chikatto.Bancho.Objects
{
    public class BanchoUserPresence : ISerializable
    {
        public int Id;
        public string Name;
        public sbyte Timezone;
        public byte CountryCode; //TODO: CountryCode dictionary
        public BanchoPermissions BanchoPermissions;
        public float Longitude;
        public float Latitude;
        public int Rank;
            
        public void Serialize(SerializationWriter writer)
        {
            writer.Write(Id);
            writer.Write(Name);
            writer.Write((byte) (Timezone + 24)); 
            writer.Write(CountryCode);
            writer.Write((byte)BanchoPermissions);
            writer.Write(Longitude);
            writer.Write(Latitude);
            writer.Write(Rank);
        }
    }
}