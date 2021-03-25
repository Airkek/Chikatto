namespace Chikatto.Constants
{
    public enum MatchType : byte
    {
        Standard,
        Powerplay
    }

    public enum MatchScoringTypes : byte
    {
        Score,
        Accuracy,
        Combo,
        ScoreV2
    }

    public enum MatchTeamTypes : byte
    {
        HeadToHead,
        TagCoop,
        TeamVS,
        TagTeamVS
    }

    public enum MatchSpecialModes : byte
    {
        Empty,
        FreeMod
    }

}