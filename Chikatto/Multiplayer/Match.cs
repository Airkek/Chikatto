using System;
using System.Linq;
using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Objects;
using Chikatto.Bancho.Serialization;
using Chikatto.Constants;
using Chikatto.Extensions;
using Chikatto.Objects;
using Chikatto.Utils;

namespace Chikatto.Multiplayer
{
    public class Match : BanchoMatch
    {
        public ushort Id;
        public new int HostId => Host.Id;
        public Presence Host;
        public Channel Channel;

        public bool InProgress;

        public Slot AvailableSlot => Slots.FirstOrDefault(x => x.Status == SlotStatus.Open);

        public async Task Join(Presence user, string password)
        {
            var slot = AvailableSlot;
            if (Password != password || slot is null)
            {
                user.WaitingPackets.Enqueue(FastPackets.MatchJoinFail);
                return;
            }

            slot.Status = SlotStatus.NotReady;
            slot.User = user;
            user.Match = this;
            user.WaitingPackets.Enqueue(await FastPackets.MatchJoinSuccess(this));

            await Channel.JoinUser(user);
            await Update();
        }

        public async Task Leave(Presence user)
        {
            user.Match = null;
            await Channel.RemoveUser(user);

            if (Channel.Users.IsEmpty)
            {
                Global.Channels.Remove(Channel.TrueName);
                Global.Rooms.Remove(Id);
                await Global.OnlineManager.AddPacketToAllUsers(await FastPackets.DisposeMatch(Id));

                XConsole.Log($"Match {ToString()} ended", ConsoleColor.Green);
            }
            else
            {
                if (user.Id == HostId)
                    Host = Slots.First(x => (x.Status & SlotStatus.HasPlayer) != 0).User;
                
                var slot = Slots.FirstOrDefault(x => x.UserId == user.Id);
            
                if(slot is null)
                    return;

                slot.User = null;
                slot.Mods = Mods.NoMod;
                slot.Team = MatchTeam.Neutral;
                slot.Status = SlotStatus.Open;

                await Update();
            }
        }

        public async Task Unready()
        {
            foreach (var slot in Slots)
            {
                if(slot.Status == SlotStatus.Ready)
                    slot.Status = SlotStatus.NotReady;
            }
        }

        public async Task Update()
        {
            var packet = await FastPackets.UpdateMatch(this);
            var foreignPacket = await FastPackets.UpdateMatch(Foreign());

            foreach (var (_, user) in Channel.Users)
                user.WaitingPackets.Enqueue(packet);

            await Global.OnlineManager.AddPacketToAllUsers(foreignPacket);
        }

        public BanchoMatch Foreign()
        {
            return new Match()
            {
                InProgress = InProgress,
                Id = Id,
                Name = Name,
                Password = string.IsNullOrEmpty(Password) ? null : " ",
                Host = Host,
                Slots = Slots,
                Beatmap = Beatmap,
                BeatmapHash = BeatmapHash,
                BeatmapId = BeatmapId,
                Mode = Mode,
                Mods = Mods,
                Type = Type,
                ScoringType = ScoringType,
                TeamType = TeamType,
                FreeMod = FreeMod,
                Seed = Seed
            };
        }

        public override string ToString() => $"<{Name} ({Id})>";

        public override void Serialize(SerializationWriter writer)
        {
            writer.Write(Id);
            writer.Write(InProgress);
            base.HostId = HostId;
            base.Serialize(writer);
        }
        
        public override void Deserialize(SerializationReader reader)
        {
            base.Deserialize(reader);

            Host = Global.OnlineManager.GetById(base.HostId);
            Channel = new Channel($"#multi_{Id}", $"Multiplayer match ({Id})");
            Global.Channels[Channel.TrueName] = Channel;
        }
    }
}