using System.ComponentModel.DataAnnotations.Schema;

namespace Chikatto.Database.Models
{
    public class Stats
    {
        [Column("id")]public int Id { get; set; }
        [Column("username")] public string Username { get; set; }
        [Column("country")] public string Country { get; set; }
        [Column("current_status")] public string Status { get; set; }
        
        [Column("pp_std")] public short PPSTD { get; set; }
        [Column("avg_accuracy_std")] public float AccuracySTD { get; set; }
        [Column("ranked_score_std")] public long RankedScoreSTD { get; set; }
        [Column("playcount_std")] public int PlaycountSTD { get; set; }
        [Column("total_score_std")] public long TotalScoreSTD { get; set; }
        [Column("replays_watched_std")] public int ReplaysWatchedSTD { get; set; }
        
        [Column("pp_taiko")] public short PPTaiko { get; set; }
        [Column("avg_accuracy_taiko")] public float AccuracyTaiko { get; set; }
        [Column("ranked_score_taiko")] public long RankedScoreTaiko { get; set; }
        [Column("playcount_taiko")] public int PlaycountTaiko { get; set; }
        [Column("total_score_taiko")] public long TotalScoreTaiko { get; set; }
        [Column("replays_watched_taiko")] public int ReplaysWatchedTaiko { get; set; }
        
        [Column("pp_ctb")] public short PPCTB { get; set; }
        [Column("avg_accuracy_ctb")] public float AccuracyCTB { get; set; }
        [Column("ranked_score_ctb")] public long RankedScoreCTB { get; set; }
        [Column("playcount_ctb")] public int PlaycountCTB { get; set; }
        [Column("total_score_ctb")] public long TotalScoreCTB { get; set; }
        [Column("replays_watched_ctb")] public int ReplaysWatchedCTB { get; set; }
        
        [Column("pp_mania")] public short PPMania { get; set; }
        [Column("avg_accuracy_mania")] public float AccuracyMania { get; set; }
        [Column("ranked_score_mania")] public long RankedScoreMania { get; set; }
        [Column("playcount_mania")] public int PlaycountMania { get; set; }
        [Column("total_score_mania")] public long TotalScoreMania { get; set; }
        [Column("replays_watched_mania")] public int ReplaysWatchedMania { get; set; }
        
        
    }
}