using System.Collections.Concurrent;
using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Objects;
using Chikatto.Constants;
using Chikatto.Database.Models;
using Chikatto.Extensions;

namespace Chikatto.Objects
{
    public class Channel
    {
        public readonly string Name;
        public readonly string Topic;
        public readonly Privileges Write;
        public readonly Privileges Read;
        public readonly bool Default;
        public readonly ConcurrentDictionary<int, Presence> Users = new ();

        public Channel(DbChannel channel)
        {
            Name = channel.Name;
            Topic = channel.Topic;
            Write = channel.WritePrivileges;
            Read = channel.ReadPrivileges;
            Default = channel.AutoJoin;
        }

        public async Task JoinUser(Presence user)
        {
            if(Users.ContainsKey(user.Id))
                return;
            
            if((user.User.Privileges & Read) != Read)
                return;
            
            Users[user.Id] = user;
            user.JoinedChannels[Name] = this;
            user.WaitingPackets.Enqueue(await FastPackets.ChannelJoinSuccess(Name));
            
            var info = await GetInfoPacket();
            
            foreach (var (_, u) in Users)
                u.WaitingPackets.Enqueue(info);
        }

        public async Task RemoveUser(Presence user)
        {
            if(!Users.ContainsKey(user.Id))
                return;
            
            Users.Remove(user.Id);
            user.JoinedChannels.Remove(Name);
            var info = await GetInfoPacket();
            
            user.WaitingPackets.Enqueue(await FastPackets.ChannelKick(Name));
            user.WaitingPackets.Enqueue(info);
            
            foreach (var (_, u) in Users)
                u.WaitingPackets.Enqueue(info);
        }

        public async Task WriteMessage(Presence user, BanchoMessage message)
        {
            if(!Users.ContainsKey(user.Id))
                return;
            
            if((user.User.Privileges & Write) != Write)
                return;
            
            var packet = await FastPackets.SendMessage(message);

            foreach (var (_, u) in Users)
            {
                if(u.Name == message.From)
                    continue;
                
                u.WaitingPackets.Enqueue(packet);
            }
        }

        public Task<Packet> GetInfoPacket() => FastPackets.ChannelInfo(this);

        public override string ToString() => $"<{Name}>";
    }
}