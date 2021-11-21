using System.Linq;

namespace Chikatto.Utils
{
    public static class TimeHelper
    {
        public static int ParseTime(string timeStr)
        {
            var timeType = new string(timeStr.Where(char.IsLetter).ToArray());
            var timeCount = int.Parse('0' + new string(timeStr.Where(char.IsDigit).ToArray()));

            var timeTotalSeconds = 0;
            
            switch (timeType)
            {
                case "sec":
                case "seconds":
                case "s":
                    timeTotalSeconds = timeCount;
                    break;
                
                case "min":
                case "minutes":
                case "m":
                    timeTotalSeconds = timeCount * 60;
                    break;
                
                case "hours":
                case "hr":
                case "h":
                    timeTotalSeconds = timeCount * 60 * 60;
                    break;
                
                case "days":
                case "d":
                case "day":
                    timeTotalSeconds = timeCount * 60 * 60 * 24;
                    break;
                
                case "month":
                case "months":
                case "mo":
                    timeTotalSeconds = timeCount * 60 * 60 * 24 * 31;
                    break;
                
                case "year":
                case "years":
                case "y":
                    timeTotalSeconds = timeCount * 60 * 60 * 24 * 365;
                    break;
            }

            return timeTotalSeconds;
        }
    }
}