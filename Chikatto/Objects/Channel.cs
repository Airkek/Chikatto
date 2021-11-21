using System.Collections.Concurrent;
using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Objects;
using Chikatto.Enums;
using Chikatto.Database.Models;
using Chikatto.Extensions;

namespace Chikatto.Objects
{
    public class Channel
    {
        public readonly string TrueName;

        public string Name
        {
            get
            {
                if (TrueName.StartsWith("#multi_"))
                    return "#multiplayer";
                if (TrueName.StartsWith("#spectator_"))
                    return "#spectator";

                return TrueName;
            }
        }
        
        public readonly string Topic;
        public readonly Privileges Write;
        public readonly Privileges Read;
        public readonly ConcurrentDictionary<int, Presence> Users = new ();

        public readonly bool IsLobby;
        public readonly bool IsTemp;

        public Channel(string name, string topic)
        {
            TrueName = name;
            Topic = topic;
            Write = Privileges.Normal;
            Read = Privileges.Normal;
            IsTemp = true;
        }
        public Channel(DbChannel channel)
        {
            TrueName = channel.Name;
            Topic = channel.Topic;
            Write = channel.PublicWrite ? Privileges.Normal : Privileges.Staff;
            Read = channel.PublicRead ? Privileges.Normal : Privileges.Staff;
            IsLobby = Name == "#lobby";
            IsTemp = false;
        }

        public async Task JoinUser(Presence user)
        {
            if(Users.ContainsKey(user.Id))
                return;
            
            if((user.User.Privileges & Read) == 0)
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

        public async Task WriteMessage(string body, string sender, int senderId)
        {
            var message = new BanchoMessage
            {
                From = sender,
                ClientId = senderId,
                Body = body,
                To = Name
            };

            var packet = await FastPackets.SendMessage(message);

            foreach (var (_, u) in Users)
            {
                if(u.Name == sender)
                    continue;
                
                u.WaitingPackets.Enqueue(packet);
            }
        }

        public async Task WriteMessage(string body, User user)
        {
            if (user.Id != Global.Bot.Id)
            {
                if(!Users.ContainsKey(user.Id))
                    return;
            
                if((user.Privileges & Write) == 0)
                    return;
            }
            
            await WriteMessage(body, user.Name, user.Id);
        }

        public Task WriteMessage(string body, Presence user) => WriteMessage(body, user.User);

        public Task<Packet> GetInfoPacket() => FastPackets.ChannelInfo(this);

        public override string ToString() => $"<{TrueName}>";
    }
}