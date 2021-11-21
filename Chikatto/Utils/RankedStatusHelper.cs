using Chikatto.Enums;

namespace Chikatto.Utils
{
    public static class RankedStatusHelper
    {
        public static int ConvertToCheesegull(DirectRankedStatus status)
        {
            return status switch
            {
                DirectRankedStatus.Played => 1,
                DirectRankedStatus.Ranked => 1,
                DirectRankedStatus.Graveyard => 0,
                DirectRankedStatus.Pending => 0,
                DirectRankedStatus.Qualified => 3,
                DirectRankedStatus.Loved => 4,
                _ => 0
            };
        }
        
        public static int ConvertToCheesegull(RankedStatus status)
        {
            return status switch
            {
                RankedStatus.Unknown => 0,
                RankedStatus.NotSubmitted => 0,
                RankedStatus.Pending => 0,
                RankedStatus.NeedUpdate => 0,
                RankedStatus.Ranked => 1,
                RankedStatus.Approved => 2,
                RankedStatus.Qualified => 3,
                RankedStatus.Loved => 4,
                _ => 0,
            };
        }
    }
}