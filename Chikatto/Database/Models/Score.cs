using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chikatto.Database.Models
{
    public class Score
    {
        [Column("id")] public int Id { get; set; }
        [Column("map_md5")] public string BeatmapChecksum { get; set; }
        [Column("userid")] public int UserId { get; set; }
        [Column("score")] public int GameScore { get; set; }
        [Column("status")] public int Completed { get; set; }
        [Column("max_combo")] public int MaxCombo { get; set; }
        [Column("mods")] public int Mods { get; set; }
        [Column("n300")] public int Count300 { get; set; }
        [Column("n100")] public int Count100 { get; set; }
        [Column("n50")] public int Count50 { get; set; }
        [Column("ngeki")] public int CountGeki { get; set; }
        [Column("nkatu")] public int CountKatu { get; set; }
        [Column("nmiss")] public int CountMiss { get; set; }
        [Column("play_time")] public DateTime Time { get; set; } 
        [Column("mode")] public int PlayMode { get; set; }
        [Column("acc")] public double Accuracy { get; set; }
        [Column("pp")] public float Performance { get; set; }
    }
}