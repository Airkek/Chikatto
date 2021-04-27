using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chikatto.Database;
using Chikatto.Database.Models;
using Dapper;

namespace Chikatto.Objects
{
    public class BeatmapManager
    {
        private readonly ConcurrentDictionary<int, string> Md5s = new(); // <Map_id, Map_md5>
        private readonly ConcurrentDictionary<string, int> SetIds = new(); // <Map_md5, Set_id>
        private readonly ConcurrentDictionary<string, Beatmap> Maps = new(); // <Map_md5, Beatmap>
        private readonly ConcurrentDictionary<int, BeatmapSet> Sets = new(); // <Set_id, BeatmapSet>

        public async Task<Beatmap> FromDb(string md5)
        {
            if (Maps.ContainsKey(md5))
                return Maps[md5];

            var map = await Db.FetchOne<Beatmap>("SELECT * FROM beatmaps WHERE beatmap_md5 = @md5", new {md5});
            
            if (map is null)
                map = await FromOsuApi(md5);
            
            var set = await BeatmapSet.FromDbByBeatmap(map);
            Cache(set);

            return map;
        }

        public async Task<Beatmap> FromDb(int id)
        {
            if (Md5s.ContainsKey(id) && Maps.ContainsKey(Md5s[id]))
                return await FromDb(Md5s[id]);

            var map = await Db.FetchOne<Beatmap>("SELECT * FROM beatmaps WHERE beatmap_id = @id", new {id});

            if (map is null)
                map = await FromOsuApi(id);

            var set = await BeatmapSet.FromDbByBeatmap(map);
            Cache(set);

            return map;
        }

        public BeatmapSet GetMapsetCache(int setId) => Sets.ContainsKey(setId) ? Sets[setId] : null;

        public void Cache(Beatmap map)
        {
            if (map is null) 
                return;
            
            Md5s[map.Id] = map.Checksum;
            SetIds[map.Checksum] = map.SetId;
            Maps[map.Checksum] = map;
        }

        public void Cache(IEnumerable<Beatmap> maps)
        {
            if (maps is null)
                return;

            foreach (var map in maps)
                Cache(map);
        }
        
        public void Cache(BeatmapSet mapset)
        {
            if (mapset is null)
                return;
            
            Sets[mapset.Id] = mapset;
            Cache(mapset.Maps);
        }

        public async Task<Beatmap> FromOsuApi(string md5)
        {
            throw new NotImplementedException();
        }
        
        public async Task<Beatmap> FromOsuApi(int id)
        {
            throw new NotImplementedException();
        }
    }
}