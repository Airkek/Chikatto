using System.Collections.Generic;
using Chikatto.Enums;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Newtonsoft.Json;

namespace Chikatto.Cheesegull.Models
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