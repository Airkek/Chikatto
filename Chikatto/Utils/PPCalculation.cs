using System.Threading.Tasks;
using Chikatto.Database.Models;

namespace Chikatto.Utils
{
    public static class PPCalculation
    {
        public static async Task Calculate(Score score, Beatmap beatmap)
        {
            score.Performance = 727; // TODO: calculation
        }
    }
}