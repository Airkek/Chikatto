using System;
using System.ComponentModel.DataAnnotations.Schema;
using Chikatto.Enums;

namespace Chikatto.Database.Models
{
    public class Score
    {
        [Column("id")] public int Id { get; set; }
        [Column("beatmap_md5")] public string BeatmapChecksum { get; set; }
        [Column("userid")] public int UserId { get; set; }
        [Column("score")] public long GameScore { get; set; }
        [Column("completed")] public int Completed { get; set; }
        [Column("max_combo")] public int MaxCombo { get; set; }
        [Column("mods")] public Mods Mods { get; set; }
        [Column("300_count")] public int Count300 { get; set; }
        [Column("100_count")] public int Count100 { get; set; }
        [Column("50_count")] public int Count50 { get; set; }
        [Column("gekis_count")] public int CountGeki { get; set; }
        [Column("katus_count")] public int CountKatu { get; set; }
        [Column("misses_count")] public int CountMiss { get; set; }
        [Column("playtime")] public int PlayTime { get; set; }
        [Column("time")] public DateTime Time { get; set; }
        [Column("play_mode")] public int PlayMode { get; set; }
        [Column("accuracy")] public double Accuracy { get; set; }
        [Column("pp")] public double Performance { get; set; }
        [Column("full_combo")] public bool Perfect { get; set; }
        [Column("is_relax")] public bool IsRelax { get; set; }
    }
}