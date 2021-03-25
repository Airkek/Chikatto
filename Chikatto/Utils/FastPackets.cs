using System;
using Chikatto.Bancho;
using Chikatto.Bancho.Enums;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PatternContexts;

namespace Chikatto.Utils
{
    public static class FastPackets
    {
        public static Packet Notification(string text)
        {
            using var packet = new WriteablePacket(PacketType.BanchoNotification);
            packet.Writer.Write(text);
            return packet.Dump();
        }

        public static Packet UserId(int id) => new (PacketType.BanchoUserId, BitConverter.GetBytes(id));
        public static Packet ServerRestart(int ms) => new(PacketType.BanchoServerRestart, BitConverter.GetBytes(ms));
    }
}