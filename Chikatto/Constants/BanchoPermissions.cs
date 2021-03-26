using System;

namespace Chikatto.Constants
{
    [Flags]
    public enum BanchoPermissions : byte
    {
        None = 0,
        Normal = 1 << 0,
        Nominator = 1 << 1,
        Supporter = 1 << 2,
        Moderator = 1 << 3,
        Developer = 1 << 4,
        Tournament = 1 << 5,
        
        Bot = Normal | Nominator | Developer,
        User = Normal | Supporter
    }
}