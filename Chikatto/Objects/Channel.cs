using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Objects;
using Chikatto.Constants;
using Chikatto.Database.Models;
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
        public ConcurrentStack<int> Users = new ();

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
            if(Users.Contains(user.Id))
                return;
            
            Users.Push(user.Id);
            user.WaitingPackets.Enqueue(FastPackets.ChannelJoinSuccess(Name));

            var temp = Users.AsEnumerable();
            var info = GetInfoPacket();
            
            foreach (var i in temp)
            {
                var u = Global.OnlineManager.GetById(i);
                u.WaitingPackets.Enqueue(info);
            }
        }

        public void WriteMessage(Presence user, BanchoMessage message)
        {
            if((user.User.Privileges & Write) != Write)
                return;
            
            var temp = Users.AsEnumerable();
            var packet = FastPackets.SendMessage(message);

            foreach (var i in temp)
            {
                var u = Global.OnlineManager.GetById(i);
                u.WaitingPackets.Enqueue(packet);
            }
        }

        public Packet GetInfoPacket() => FastPackets.ChannelInfo(Name, Topic, Users.Count);

        public override string ToString() => $"<{Name}>";
    }
}