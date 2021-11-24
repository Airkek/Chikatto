namespace Chikatto.Enums
{
    public enum RankedStatus : sbyte
    {
        Unknown = -2,
        NotSubmitted = -1,
        Pending = 0,
        NeedUpdate = 1,
        Ranked = 2,
        Approved = 3,
        Qualified = 4,
        Loved = 5
    }

    public enum RankedStatusOsuApi : sbyte
    {
        Graveyard = -2,
        WIP = -1,
        Pending = 0,
        Ranked = 1,
        Approved = 2,
        Qualified = 3,
        Loved = 4
    }
}