using System;
using System.Linq;
using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Objects;
using Chikatto.Bancho.Serialization;
using Chikatto.Enums;
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

        public int NeedLoad = 0;

        public bool InProgress;

        public string Url => $"osump://{Id}/{Password}";
        public string Embed => $"[{Url} {Name}";

        public Slot AvailableSlot => Slots.FirstOrDefault(x => x.Status == SlotStatus.Open);

        public Slot GetSlot(int id) => Slots.FirstOrDefault(x => x.UserId == id);

        public async Task UpdateUserStatus(Presence user, SlotStatus status)
        {
            var slot = user.Match.GetSlot(user.Id);
            
            if(slot is null)
                return;

            slot.Status = status;
            await Update();
        }

        public async Task Start()
        {
            var ready = Slots.Where(x => (x.Status & SlotStatus.HasPlayer) != 0 && x.Status != SlotStatus.NoMap);
            
            foreach (var slot in ready)
            {
                NeedLoad++;
                slot.Status = SlotStatus.Playing;
                slot.Skipped = false;
            }

            InProgress = true;
            
            var packet = await FastPackets.MatchStart(this);

            foreach (var slot in ready)
                slot.User.WaitingPackets.Enqueue(packet);
        }
        
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
                var slot = GetSlot(user.Id);
            
                if(slot is null)
                    return;

                slot.User = null;
                slot.Mods = Mods.NoMod;
                slot.Team = MatchTeam.Neutral;
                slot.Status = SlotStatus.Open;
                
                if (user.Id == HostId)
                    Host = Slots.First(x => (x.Status & SlotStatus.HasPlayer) != 0).User;

                await Update();
            }
        }

        public async Task AddPacketsToAllPlayers(Packet packet)
        {
            foreach (var (_, user) in Channel.Users)
                user.WaitingPackets.Enqueue(packet);
        }
        
        public async Task AddPacketsToSpecificPlayers(Packet packet, SlotStatus status = SlotStatus.Playing)
        {
            foreach (var slot in Slots.Where(x => x.Status == status))
                slot.User.WaitingPackets.Enqueue(packet);
        }

        public async Task Unready(SlotStatus status = SlotStatus.Ready)
        {
            foreach (var slot in Slots)
            {
                if ((slot.Status & status) != 0)
                    slot.Status = SlotStatus.NotReady;
            }
        }

        public async Task Update()
        {
            var packet = await FastPackets.UpdateMatch(this);
            var foreignPacket = await FastPackets.UpdateMatch(Foreign());

            await AddPacketsToAllPlayers(packet);
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

        public override string ToString() => Channel.ToString();

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

            Id = ++Global.MatchId;
            Host = Global.OnlineManager.GetById(base.HostId);
            Channel = new Channel($"#multi_{Id}", $"Multiplayer match ({Id})");
            Global.Channels[Channel.TrueName] = Channel;
        }
    }
}