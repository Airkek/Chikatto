using System;

namespace Chikatto.Constants
{
    public enum MatchType : byte
    {
        Standard,
        Powerplay
    }

    public enum MatchScoringType : byte
    {
        Score,
        Accuracy,
        Combo,
        ScoreV2
    }

    public enum MatchTeamType : byte
    {
        HeadToHead,
        TagCoop,
        TeamVS,
        TagTeamVS
    }

    public enum MatchTeam : byte
    {
        Neutral,
        Blue,
        Red
    }

    [Flags]
    public enum SlotStatus : byte
    {
        Open = 1 << 0,
        Locked = 1 << 1,
        NotReady = 1 << 2,
        Ready = 1 << 3,
        NoMap = 1 << 4,
        Playing = 1 << 5,
        Complete = 1 << 6,
        Quit = 1 << 7,
        
        HasPlayer = NotReady | Ready | NoMap | Playing | Complete
    }
}