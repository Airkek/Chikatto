using System;
using Chikatto.Bancho.Enums;

namespace Chikatto.Events
{
    [AttributeUsage(AttributeTargets.Method)]
    public class EventAttribute : Attribute
    {
        public PacketType Type;
        public bool Restricted;

        public EventAttribute(PacketType type, bool restricted = true)
        {
            Type = type;
            Restricted = true;
        }
    }
}