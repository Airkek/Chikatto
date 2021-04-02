using System.Collections.Concurrent;
using Chikatto.Bancho.Serialization;
using Chikatto.Constants;
using Chikatto.Multiplayer;

namespace Chikatto.Bancho.Objects
{
    public class BanchoMatch : ISerializable, IDeserializable
    {
        public string Name;
        public string Password;
        public int HostId;
        public ConcurrentBag<Slot> Slots = new ConcurrentBag<Slot>
        {
            new(), new(), new(), new(),
            new(), new(), new(), new(),
            new(), new(), new(), new(),
            new(), new(), new(), new()
        };

        public string Beatmap;
        public int BeatmapId;
        public string BeatmapHash;

        public MatchType Type;
        public Mods Mods;
        public GameMode Mode;
        public MatchScoringType ScoringType;
        public MatchTeamType TeamType;
        public bool FreeMod;

        public int Seed;
        
        public virtual void Serialize(SerializationWriter writer)
        {
            writer.Write((byte) Type);
            writer.Write((int) Mods);
            writer.Write(Name);
            writer.Write(Password);
            writer.Write(Beatmap);
            writer.Write(BeatmapId);
            writer.Write(BeatmapHash);
            
            foreach (var slot in Slots)
                writer.Write((byte) slot.Status);
            
            foreach (var slot in Slots)
                writer.Write((byte) slot.Team);

            foreach (var slot in Slots)
            {
                if((slot.Status & SlotStatus.HasPlayer) != 0)
                    writer.Write(slot.UserId);
            }

            writer.Write(HostId);
            writer.Write((byte) Mode);
            writer.Write((byte) ScoringType);
            writer.Write((byte) TeamType);
            writer.Write(FreeMod);

            if (FreeMod)
            {
                foreach (var slot in Slots)
                {
                    writer.Write((int) slot.Mods);
                }
            }
            
            writer.Write(Seed);
        }

        public virtual void Deserialize(SerializationReader reader)
        {
            reader.ReadInt16(); //id
            reader.ReadByte(); //in progress
            
            Type = (MatchType) reader.ReadByte();
            Mods = (Mods) reader.ReadInt32();
            
            Name = reader.ReadString();
            Password = reader.ReadString();
            
            Beatmap = reader.ReadString();
            BeatmapId = reader.ReadInt32();
            BeatmapHash = reader.ReadString();

            foreach (var slot in Slots)
                slot.Status = (SlotStatus) reader.ReadByte();
            
            foreach (var slot in Slots)
                slot.Team = (MatchTeam) reader.ReadByte();
            
            foreach (var slot in Slots)
            {
                if ((slot.Status & SlotStatus.HasPlayer) != 0)
                    reader.ReadInt32(); //player id
            }

            HostId = reader.ReadInt32();
            Mode = (GameMode) reader.ReadByte();
            ScoringType = (MatchScoringType) reader.ReadByte();
            TeamType = (MatchTeamType) reader.ReadByte();
            FreeMod = reader.ReadBoolean();

            if (FreeMod)
            {
                foreach (var slot in Slots)
                    slot.Mods = (Mods) reader.ReadInt32();
            }

            Seed = reader.ReadInt32();
        }
    }
}