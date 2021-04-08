using System;

namespace Chikatto.Enums
{
    [Flags]
    public enum BanchoPermissions : byte
    {
        None = 0,
        Normal = 1 << 0,
        BAT = 1 << 1,
        Supporter = 1 << 2,
        Moderator = 1 << 3,
        Peppy = 1 << 4,
        Tournament = 1 << 5,
        
        Bot = Normal | BAT | Peppy,
    }
}