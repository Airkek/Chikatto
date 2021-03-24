namespace Chikatto.Bancho.Enums
{
    public enum OsuType
    {
        #region Integer
        Int8 = 0,
        UInt8 = 1,
        Int16 = 2,
        UInt16 = 3,
        Int32 = 4,
        UInt32 = 5,
        FInt32 = 6,
        Int64 = 7,
        UInt64 = 8,
        FInt64 = 9,
        #endregion

        #region Osu
        Message = 11,
        Channel = 12,
        Match = 13,
        ScoreFrame = 14,
        MapInfoRequest = 15,
        MapInfoReply = 16,
        #endregion

        #region Misc
        Int32List2L = 17,
        Int32List4L = 18,
        String = 19,
        Raw = 20
        #endregion
    }
}