using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Chikatto.Enums;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Textures;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Formats;
using osu.Game.Beatmaps.Legacy;
using osu.Game.IO;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Catch;
using osu.Game.Rulesets.Mania;
using osu.Game.Rulesets.Osu;
using osu.Game.Rulesets.Taiko;
using osu.Game.Scoring;
using osu.Game.Skinning;
using Beatmap = Chikatto.Database.Models.Beatmap;
using Score = Chikatto.Database.Models.Score;

namespace Chikatto.Utils
{
    public static class PPCalculation
    {
        public static async Task Calculate(Score score, Beatmap beatmap)
        {
            Ruleset ruleset = score.PlayMode switch
            {
                GameMode.Standard => new OsuRuleset(),
                GameMode.Taiko => new TaikoRuleset(),
                GameMode.Catch => new CatchRuleset(),
                GameMode.Mania => new ManiaRuleset(),
                _ => new OsuRuleset()
            };

            var mods = ruleset.ConvertFromLegacyMods(score.Mods & ~LegacyMods.Relax);

            var scoreInfo = new ScoreInfo
            {
                Accuracy = score.Accuracy / 100f,
                MaxCombo = score.MaxCombo,
                TotalScore = score.GameScore,
                Mods = mods.ToArray()
            };

            var wbc = new WorkingBeatmapC(Path.Combine(".data", "maps", beatmap.FileName));

            var calculator = ruleset.CreatePerformanceCalculator(wbc, scoreInfo);

            score.Performance = calculator.Calculate();
        }
    }
    
    public class WorkingBeatmapC : WorkingBeatmap
    {
        private readonly osu.Game.Beatmaps.Beatmap _beatmap;

        public WorkingBeatmapC(string file, int? beatmapId = null)
            : this(ReadFromFile(file), beatmapId)
        {
        }

        private WorkingBeatmapC(osu.Game.Beatmaps.Beatmap beatmap, int? beatmapId = null)
            : base(beatmap.BeatmapInfo, null)
        {
            _beatmap = beatmap;

            beatmap.BeatmapInfo.Ruleset = (GameMode)beatmap.BeatmapInfo.RulesetID switch
            {
                GameMode.Standard => new OsuRuleset().RulesetInfo,
                GameMode.Taiko => new CatchRuleset().RulesetInfo,
                GameMode.Catch => new TaikoRuleset().RulesetInfo,
                GameMode.Mania => new ManiaRuleset().RulesetInfo,
                _ => new OsuRuleset().RulesetInfo,
            };

            if (beatmapId.HasValue)
                beatmap.BeatmapInfo.OnlineID = beatmapId;
        }

        private static osu.Game.Beatmaps.Beatmap ReadFromFile(string filename)
        {
            using var stream = File.OpenRead(filename);
            using var streamReader = new LineBufferedReader(stream);

            return Decoder.GetDecoder<osu.Game.Beatmaps.Beatmap>(streamReader).Decode(streamReader);
        }

        protected override IBeatmap GetBeatmap() => _beatmap;
        protected override Texture GetBackground() => null;
        protected override Track GetBeatmapTrack() => null;
        protected override ISkin GetSkin() => null;

        public override Stream GetStream(string storagePath) => null;
    }
}