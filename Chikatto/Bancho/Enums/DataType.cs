namespace Chikatto.Bancho.Enums
{
    public enum DataType
    {
        #region Integer
        Short,
        UShort,
        Int,
        UInt,
        Float,
        Long,
        ULong,
        Double,
        #endregion

        #region Osu
        Message,
        Channel,
        Match,
        ScoreFrame,
        MapInfoRequest,
        MapInfoReply,
        #endregion

        #region Misc
        IntList2L,
        IntList4L,
        String,
        Raw
        #endregion
    }
}