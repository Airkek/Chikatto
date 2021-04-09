using System.Collections.Generic;

namespace Chikatto.Utils.Cheesegull.Models
{
    public class BeatmapSet
    {
        public int SetId;
        public string Artist;
        public string Title;
        public string Creator;
        public string Source;
        public bool HasVideo;

        public List<Beatmap> ChildrenBeatmaps;

        public int RankedStatus;
        
        public string Tags;
        public string LastUpdate;
    }
}