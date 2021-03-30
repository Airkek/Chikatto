using System.Collections.Concurrent;
using Chikatto.Bancho;
using Chikatto.Bancho.Objects;
using Chikatto.Constants;
using Chikatto.Database.Models;
using Chikatto.Extensions;
using Chikatto.Utils;

namespace Chikatto.Objects
{
    public class Channel
    {
        public string Name;
        public string Topic;
        public Privileges Write;
        public Privileges Read;
        public bool Default;
        public ConcurrentDictionary<int, Presence> Users = new ();

        public Channel(DbChannel channel)
        {
            Name = channel.Name;
            Topic = channel.Topic;
            Write = channel.WritePrivileges;
            Read = channel.ReadPrivileges;
            Default = channel.AutoJoin;
        }

        public void JoinUser(Presence user)
        {
            if(Users.ContainsKey(user.Id))
                return;
            
            if((user.User.Privileges & Read) != Read)
                return;
            
            Users[user.Id] = user;
            user.JoinedChannels[Name] = this;
            user.WaitingPackets.Enqueue(FastPackets.ChannelJoinSuccess(Name));
            
            var info = GetInfoPacket();
            
            foreach (var (_, u) in Users)
                u.WaitingPackets.Enqueue(info);
        }

        public void RemoveUser(Presence user)
        {
            if(!Users.ContainsKey(user.Id))
                return;
            
            Users.Remove(user.Id);
            user.JoinedChannels.Remove(Name);
            var info = GetInfoPacket();
            
            user.WaitingPackets.Enqueue(FastPackets.ChannelKick(Name));
            user.WaitingPackets.Enqueue(info);
            
            foreach (var (_, u) in Users)
                u.WaitingPackets.Enqueue(info);
        }

        public void WriteMessage(Presence user, BanchoMessage message)
        {
            if(!Users.ContainsKey(user.Id))
                return;
            
            if((user.User.Privileges & Write) != Write)
                return;
            
            var packet = FastPackets.SendMessage(message);

            foreach (var (_, u) in Users)
            {
                if(u.Name == message.From)
                    continue;
                
                u.WaitingPackets.Enqueue(packet);
            }
        }

        public Packet GetInfoPacket() => FastPackets.ChannelInfo(Name, Topic, Users.Count);

        public override string ToString() => $"<{Name}>";
    }
}