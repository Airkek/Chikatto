using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chikatto.Database;
using Chikatto.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Chikatto.Enums;
using Chikatto.Objects;
using Chikatto.Utils;
using Chikatto.Utils.Cheesegull;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;

namespace Chikatto.Controllers
{
    [Route("/web/")]
    public class WebController : Controller
    {
        #region UselessShit
        
        [Route("check-updates.php")]
        public async Task<IActionResult> CheckUpdates() => Redirect("https://old.ppy.sh/web/check-updates.php" + Request.QueryString);

        [HttpPost("osu-session.php")]
        public async Task<IActionResult> OsuSession() => NotFound("NotImplemented");

        [Route("osu-getseasonal.php")]
        public async Task<IActionResult> GetSeasonalBgs() => Ok(Global.Config.SeasonalBgsJson);

        [Route("bancho_connect.php")]
        public async Task<IActionResult> BanchoConnect() => Ok("Chikatto");
        
        #endregion

        #region ScoreSubmission

        [HttpPost("osu-submit-modular.php")]
        [HttpPost("osu-submit-modular-selector.php")]
        public async Task<IActionResult> SubmitModular()
        {
            var isSelector = Request.Path.ToString().EndsWith("-selector.php");

            if (!isSelector)
                return Ok("error: oldver"); // should I accept fallback?
            else if (!Request.Headers.ContainsKey("Token"))
                return Ok("error: oldver"); // patched or too old client with no osu!auth.dll 

            string[] requiredArgs = {"x", "ft", "score", "fs", "bmk", "iv", "c1", "st", "pass", "osuver", "s"};

            if (requiredArgs.Any(arg => !Request.Form.ContainsKey(arg)))
                return Ok("error: no");

            var dataB64 = (string) Request.Form["score"];
            var ivB64 = (string) Request.Form["iv"];
            var osuver = (string) Request.Form["osuver"];
            var passwordMd5 = (string) Request.Form["pass"];
            var failed = Request.Form["x"] == "1";

            var scoreData = await Submission.Decrypt(dataB64, ivB64, osuver);
            var score = await Submission.ScoreDataToScore(scoreData);

            var username = scoreData[1].TrimEnd(); // remove anticheat flags

            var user = await GetPresence(username, passwordMd5);

            if (user is null)
                return Ok("error: pass");

            if (user.Restricted)
                return Ok("error: ban");

            var map = await Global.BeatmapManager.FromDb(score.BeatmapChecksum);

            if (map is null)
                return Ok("error: no");

            await PPCalculation.Calculate(score, map);
            
            var oldBest = await Db.FetchOne<Score>(
                "SELECT * FROM scores WHERE userid = @uid AND beatmap_md5 = @bmMd5 AND is_relax = @isRelax ORDER BY pp DESC LIMIT 1",
                new { uid = user.Id, bmMd5 = score.BeatmapChecksum, isRelax = score.IsRelax });

            var oldMapRank = oldBest is null ? 0 :
                await Db.FetchOne<int>("SELECT COUNT(*) FROM scores WHERE beatmap_md5 = @bmMd5 AND is_relax = @isRelax AND pp > @pp",
                    new { bmMd5 = score.BeatmapChecksum, isRelax = score.IsRelax, pp = oldBest.Performance});

            var oldMC = await Db.FetchOne<int>("SELECT max_combo FROM scores WHERE userid = @uid AND is_relax = @isRelax ORDER BY max_combo DESC LIMIT 1",
                new { isRelax = score.IsRelax, uid = user.Id });
            
            var oldRank = user.Rank;
            var oldRS = user.RankedScore;
            var oldTS = user.TotalScore;
            var oldAcc = user.Accuracy;
            var oldPP = user.PP;
            
            var newMapRank = await Db.FetchOne<int>("SELECT COUNT(*) FROM scores WHERE beatmap_md5 = @bmMd5 AND is_relax = @isRelax AND pp > @pp",
                new { bmMd5 = score.BeatmapChecksum, isRelax = score.IsRelax, pp = score.Performance});

            await user.UpdateStats();
            
            var newMC = await Db.FetchOne<int>("SELECT max_combo FROM scores WHERE userid = @uid AND is_relax = @isRelax ORDER BY max_combo DESC LIMIT 1",
                new { isRelax = score.IsRelax, uid = user.Id });
            
            XConsole.Log(score.Accuracy.ToString());
            
            var charts = new[]
            {
                $"beatmapId:{map.MapId}",
                $"beatmapSetId:{map.SetId}",
                $"beatmapPlaycount:{map.Playcount}",
                $"beatmapPasscount:{map.Passcount}",
                "approvedDate:0",
                "\n",
                "chartId:beatmap",
                "chartUrl:localhost:5000",
                "chartName:Beatmap Ranking",
                Submission.ChartEntry("rank", oldMapRank == 0 ? "" : oldMapRank.ToString(), newMapRank.ToString()),
                Submission.ChartEntry("rankedScore", oldBest?.GameScore.ToString(), score.GameScore.ToString()),
                Submission.ChartEntry("totalScore", oldBest?.GameScore.ToString(), score.GameScore.ToString()),
                Submission.ChartEntry("maxCombo", oldBest?.MaxCombo.ToString(), score.MaxCombo.ToString()),
                Submission.ChartEntry("accuracy", oldBest?.Accuracy.ToString("F").Replace(',', '.'), score.Accuracy.ToString("F").Replace(',', '.')),
                Submission.ChartEntry("pp", oldBest?.Performance.ToString().Split(',')[0], score.Performance.ToString().Split(',')[0]),
                "onlineScoreId:1",
                "\n",
                "chartId:overall",
                $"chartUrl:https://{Global.Config.Domain}/u/{user.Id}",
                "chartName:Overall Ranking",
                Submission.ChartEntry("rank", oldRank.ToString(), user.Rank.ToString()),
                Submission.ChartEntry("rankedScore", oldRS.ToString(), user.RankedScore.ToString()),
                Submission.ChartEntry("totalScore", oldTS.ToString(), user.TotalScore.ToString()),
                Submission.ChartEntry("maxCombo", oldMC.ToString(), newMC.ToString()),
                Submission.ChartEntry("accuracy", oldAcc.ToString("F").Replace(',', '.'), user.Accuracy.ToString("F").Replace(',', '.')),
                Submission.ChartEntry("pp", oldPP.ToString(), user.PP.ToString()),
                "achievements-new:"
            };

            return Ok(string.Join("|", charts)); //TODO
        }

        #endregion
        
        #region osu!direct
        
        [HttpGet("osu-search.php")]
        public async Task<IActionResult> DirectSearch()
        {
            if (!await CheckAuthorization() || !int.TryParse(Request.Query["p"], out var offset)
                                            || !int.TryParse(Request.Query["m"], out var mode) 
                                            || !int.TryParse(Request.Query["r"], out var _status))
            {
                return BadRequest();
            }

            var status = (DirectRankedStatus) _status;

            var query = string.Empty;

            if (Request.Query["q"] != "Newest" && Request.Query["q"] != "Top+Rated" &&
                Request.Query["q"] != "Most+Played")
            {
                query = Request.Query["q"];
            }

            var res = await CheesegullHelper.Search(offset, query, mode, status);

            var output = new List<string>
            {
                res.Length == 100 ? "101" : res.Length.ToString()
            };

            foreach (var set in res)
            {
                if(set.ChildrenBeatmaps is null || set.ChildrenBeatmaps.Count == 0)
                    continue;
                
                set.ChildrenBeatmaps.Sort((x, y) => x.DifficultyRating.CompareTo(y.DifficultyRating));
                output.Add($"{set.SetId}.osz|{set.Artist}|{set.Title}|{set.Creator}|{set.RankedStatus}|10.0|{set.LastUpdate}|{set.SetId}|0|0|0|0|0|" + string.Join(",", set.ChildrenBeatmaps.Select(x => 
                    $"[{x.DifficultyRating:n2}⭐] {x.DiffName} {{CS{x.CS} OD{x.OD} AR{x.AR} HP{x.HP}}}@{(int) x.Mode}")));
            }
            
            return Ok(string.Join("\r\n", output));
        }

        [HttpGet("osu-search-set.php")]
        public async Task<IActionResult> SetSearch()
        {
            if (!await CheckAuthorization())
                return BadRequest();


            Utils.Cheesegull.Models.BeatmapSet set = null;
            
            if (Request.Query.ContainsKey("s"))
            {
                if (!int.TryParse(Request.Query["s"], out var s))
                    return BadRequest();
                
                set = await CheesegullHelper.GetSet(s);
            }
            else if (Request.Query.ContainsKey("b"))
            {
                if (!int.TryParse(Request.Query["b"], out var b))
                    return BadRequest();
                
                var map = await CheesegullHelper.GetMap(b);

                set = await CheesegullHelper.GetSet(map.ParentSetID);
            }

            if (set is null)
                return NotFound();
            
            return Ok($"{set.SetId}.osz|{set.Artist}|{set.Title}|{set.Creator}|{set.RankedStatus}|10.0|0|{set.SetId}|0|0|0|0|0");
        }
        
        #endregion

        //TODO: /web/ 

        public async Task<Presence> GetPresence(string u, string h)
        {
            var presence = await Auth.Login(u, h);

            if (presence is null || !presence.Online)
                XConsole.Log($"{presence?.ToString() ?? u} requested {Request.Path} while not online", ConsoleColor.Yellow);

            return presence;
        }
        
        public async Task<Presence> GetPresence()
        {
            var user = Auth.GetSafeName(Request.Query["u"]);
            var pwMd5 = (string) Request.Query["h"];

            return await GetPresence(user, pwMd5);
        }

        public async Task<bool> CheckAuthorization() => await GetPresence() is not null;
    }
}