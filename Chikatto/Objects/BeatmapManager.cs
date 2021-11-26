using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Chikatto.Database;
using Chikatto.Database.Models;
using Chikatto.Enums;
using Chikatto.Utils;
using Dapper;
using Newtonsoft.Json.Linq;

namespace Chikatto.Objects
{
    public class BeatmapManager
    {
        private readonly ConcurrentDictionary<int, string> Md5s = new(); // <Map_id, Map_md5>
        private readonly ConcurrentDictionary<string, int> SetIds = new(); // <Map_md5, Set_id>
        private readonly ConcurrentDictionary<string, Beatmap> Maps = new(); // <Map_md5, Beatmap>
        private readonly ConcurrentDictionary<string, string> FailedMapMd5 = new(); // <Map_md5> - maps that not submitted
        private readonly ConcurrentDictionary<int, int> FailedMapId = new(); // <Map_id> - ^
        
        public async Task<Beatmap> FromDb(string md5)
        {
            if (Maps.ContainsKey(md5))
                return Maps[md5];

            return await Db.FetchOne<Beatmap>("SELECT * FROM beatmaps WHERE beatmap_md5 = @md5", new {md5}) ?? await FromOsuApi(md5);
        }

        public async Task<Beatmap> FromDb(int id)
        {
            if (Md5s.ContainsKey(id) && Maps.ContainsKey(Md5s[id]))
                return await FromDb(Md5s[id]);

            return await Db.FetchOne<Beatmap>("SELECT * FROM beatmaps WHERE beatmap_id = @id", new {id}) ?? await FromOsuApi(id);
        }

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

        private static RankedStatus osuApiToRankedStatus(RankedStatusOsuApi status)
        {
            return status switch
            {
                RankedStatusOsuApi.Ranked => RankedStatus.Ranked,
                RankedStatusOsuApi.Qualified => RankedStatus.Qualified,
                RankedStatusOsuApi.Approved => RankedStatus.Approved,
                RankedStatusOsuApi.Loved => RankedStatus.Loved,
                _ => RankedStatus.Pending
            };
        }

        private List<Beatmap> ResponseToBeatmaps(string response)
        {
            var maps = new List<Beatmap>();
            
            //TODO: write models for osu api ;//
            var jarray = JArray.Parse(response);

            foreach (dynamic dmap in jarray)
            {
                var status = osuApiToRankedStatus((RankedStatusOsuApi) int.Parse((string)dmap.approved));
                var map = new Beatmap
                {
                    Ranked = status,
                    Checksum = (string)dmap.file_md5,
                    FileName = $"{dmap.beatmap_id}.osu",
                    MapId = int.Parse((string)dmap.beatmap_id),
                    SetId = int.Parse((string)dmap.beatmapset_id),
                    Name = $"{dmap.artist} - {dmap.title} [{dmap.version}]",
                    AR = float.Parse(((string)dmap.diff_approach).Replace('.', ',')),
                    OD = float.Parse(((string)dmap.diff_overall).Replace('.', ',')),
                    Mode = (GameMode) int.Parse((string)dmap.mode),
                    MaxCombo = int.Parse((string)dmap.max_combo),
                    HitLength = int.Parse((string)dmap.hit_length),
                    BPM = (int)float.Parse(((string)dmap.bpm).Replace('.', ',')),
                    Playcount = 0,
                    Passcount = 0,
                    LatestUpdate = (int)new DateTimeOffset((DateTime) dmap.last_update).ToUnixTimeSeconds(),
                    Frozen = false,
                    DiffSTD = 0,
                    DiffMania = 0,
                    DiffTaiko = 0,
                    DiffCTB = 0,
                    DisablePP = status != RankedStatus.Approved && status != RankedStatus.Ranked
                };
                
                var starrate = float.Parse(((string)dmap.difficultyrating).Replace('.', ','));

                switch (map.Mode)
                {
                    case GameMode.Standard:
                        map.DiffSTD = starrate;
                        break;
                    case GameMode.Taiko:
                        map.DiffTaiko = starrate;
                        break;
                    case GameMode.Mania:
                        map.DiffMania = starrate;
                        break;
                    case GameMode.Catch:
                        map.DiffCTB = starrate;
                        break;
                }
                
                maps.Add(map);
            }

            return maps;
        }

        public async Task<Beatmap> FromOsuApi(string md5)
        {
            if (FailedMapMd5.ContainsKey(md5))
                return null;
            
            var res = await Request("h", md5);
            return await FromRes(res);
        }
        
        public async Task<Beatmap> FromOsuApi(int id)
        {
            if (FailedMapId.ContainsKey(id))
                return null;
            
            var res = await Request("b", id);
            return await FromRes(res);
        }

        private async Task<Beatmap> FromRes(string res)
        {
            var maps = ResponseToBeatmaps(res);
            var map = maps.FirstOrDefault();

            if (map is not null)
                await SaveAllSetToDatabase(map.SetId);

            return map;
        }

        public async Task SaveAllSetToDatabase(int setId)
        {
            XConsole.Log($"Caching set {setId}", back: ConsoleColor.Cyan);
            var res = await Request("s", setId);
            var maps = ResponseToBeatmaps(res);
            
            if(!maps.Any())
                return;

            var mapsPath = Path.Combine(".data", "maps");
            using var wc = new WebClient();
            foreach (var map in maps)
            {
                var mapPath = Path.Join(mapsPath, map.FileName);

                if (!File.Exists(mapPath))
                    await wc.DownloadFileTaskAsync(new Uri($"https://old.ppy.sh/osu/{map.MapId}"), mapPath);

                await Db.Execute(
                    "INSERT INTO beatmaps (beatmap_id, beatmapset_id, beatmap_md5, song_name, file_name, " +
                    "ar, od, mode, difficulty_std, difficulty_taiko, difficulty_ctb, difficulty_mania, " +
                    "max_combo, hit_length, bpm, ranked, latest_update, disable_pp) VALUES (" +
                    "@id, @sid, @md5, @name, @file, @ar, @od, @mode, @std, @taiko, @ctb, @mania, @mc, " +
                    "@hl, @bpm, @r, @lu, @dpp)",
                    new
                    {
                        id = map.MapId, sid = map.SetId, md5 = map.Checksum,
                        name = map.Name, file = map.FileName, ar = map.AR,
                        od = map.OD, mode = map.Mode, std = map.DiffSTD,
                        taiko = map.DiffTaiko, ctb = map.DiffCTB, mania = map.DiffMania, 
                        mc = map.MaxCombo, hl = map.HitLength, bpm = map.BPM,
                        r = map.Ranked, lu = map.LatestUpdate, dpp = map.DisablePP
                    });
                
                Cache(map);
            }
            
            XConsole.Log($"Saved {maps.Count} diffs for {maps.FirstOrDefault().Name}", back: ConsoleColor.Cyan);
        }

        private static async Task<string> Request<T>(string arg, T data)
        {
            using var wc = new WebClient();

            try
            {
                return await wc.DownloadStringTaskAsync($"https://old.ppy.sh/api/get_beatmaps?k={Global.Config.OsuApiToken}&{arg}={data}");
            }
            catch
            {
                return "[]";
            }
        }
    }
}