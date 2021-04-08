using Chikatto.Enums;

namespace Chikatto.Cheesegull.Models
{
    public class Beatmap
    {
        public int BeatmapID;
        public int ParentSetID;
        public string DiffName;
        public string FileMD5;
        public GameMode Mode;
        public float BPM;
        public float AR;
        public float OD;
        public float CS;
        public float HP;
        public int TotalLength;
        public int HitLength;
        public int PlayCount;
        public int PassCount;
        public int MaxCombo;
        public float DifficultyRating;
    }
}