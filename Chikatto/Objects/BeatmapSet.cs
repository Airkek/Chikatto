using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chikatto.Database;
using Chikatto.Database.Models;
using Chikatto.Enums;

namespace Chikatto.Objects
{
    public class BeatmapSet
    {
        public readonly int Id;
        
        public readonly string Artist;
        public readonly string Title;
        public readonly string Creator;
        public readonly RankedStatus Status;

        public readonly Beatmap[] Maps;
        public readonly int Count;

        public BeatmapSet(IEnumerable<Beatmap> maps)
        {
            var diff = maps.ElementAt(0);
            
            Artist = diff.Artist;
            Title = diff.Title;
            Creator = diff.Creator;
            Status = diff.Status;

            Id = diff.SetId;
            Maps = maps.ToArray();
            Count = Maps.Length;
        }

        public static async Task<BeatmapSet> FromDbByBeatmap(int mapId)
        {
            var map = await Global.BeatmapManager.FromDb(mapId);
            return await FromDbByBeatmap(map);
        }
        
        public static async Task<BeatmapSet> FromDbByBeatmap(string mapMd5)
        {
            var map = await Global.BeatmapManager.FromDb(mapMd5);
            return await FromDbByBeatmap(map);
        }
        
        public static Task<BeatmapSet> FromDbByBeatmap(Beatmap beatmap) => FromDb(beatmap.SetId);

        public static async Task<BeatmapSet> FromDb(int setId)
        {
            var set = Global.BeatmapManager.GetMapsetCache(setId);
            
            if (set is null)
            {
                var maps = await Db.FetchAll<Beatmap>("SELECT * FROM maps WHERE set_id = @setId", new {setId});

                if (maps is null || !maps.Any())
                    return null;

                set = new BeatmapSet(maps);
            }

            return set;
        }
    }
}