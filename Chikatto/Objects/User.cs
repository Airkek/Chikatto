using System.Collections.Generic;
using Chikatto.Bancho;

namespace Chikatto.Objects
{
    public class User
    {
        public string Name;
        public string SafeName;

        public int Id;

        public long LastPong;

        public string BanchoToken;

        public List<Packet> WaitingPackets = new();

        public override string ToString() => $"<{Name} ({Id})>";
        //TODO
    }
}