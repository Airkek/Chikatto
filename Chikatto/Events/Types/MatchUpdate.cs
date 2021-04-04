using System.Linq;
using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Enums;
using Chikatto.Bancho.Objects;
using Chikatto.Constants;
using Chikatto.Objects;

namespace Chikatto.Events.Types
{
    public class MatchUpdate
    {
        [Event(PacketType.OsuMatchChangeSettings, false)]
        public static async Task Handle(PacketReader reader, Presence user)
        {
            var match = user.Match;
            var newMatch = reader.ReadBanchoObject<BanchoMatch>();

            if (match.BeatmapHash != newMatch.BeatmapHash ||
                match.Mode != newMatch.Mode ||
                match.Type != newMatch.Type ||
                match.ScoringType != newMatch.ScoringType ||
                match.TeamType != newMatch.TeamType)
            {
                await match.Unready();
            }

            match.Beatmap = newMatch.Beatmap;
            match.BeatmapId = newMatch.BeatmapId;
            match.BeatmapHash = newMatch.BeatmapHash;
            match.Name = newMatch.Name.Length > 0 ? newMatch.Name : $"{match.Host.Name}'s game";
            match.TeamType = newMatch.TeamType;
            match.ScoringType = newMatch.ScoringType;
            match.Type = newMatch.Type;
            match.Mode = newMatch.Mode;
            match.Seed = newMatch.Seed;
            match.FreeMod = newMatch.FreeMod;

            if (match.TeamType != newMatch.TeamType)
            {
                switch (newMatch.TeamType)
                {
                    case MatchTeamType.TagTeamVS:
                    case MatchTeamType.TeamVS:
                    {
                        for (var i = 0; i < match.Slots.Count; i++)
                        {
                            var slot = match.Slots.ElementAt(i);
                            slot.Team = i % 2 == 0 ? MatchTeam.Red : MatchTeam.Blue;
                        }
                        break;
                    }

                    default:
                    {
                        foreach (var slot in match.Slots)
                            slot.Team = MatchTeam.Neutral;
                        break;
                    }
                }
            }

            await match.Update();
        }
    }
}