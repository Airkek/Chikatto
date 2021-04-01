using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chikatto.Database.Models
{
    public class Score
    {
        [Column("id")] public int Id;
        [Column("map_md5")] public string BeatmapChecksum;
        [Column("userid")] public int UserId;
        [Column("score")] public int GameScore;
        [Column("status")] public int Completed;
        [Column("max_combo")] public int MaxCombo;
        [Column("mods")] public int Mods;
        [Column("n300")] public int Count300;
        [Column("n100")] public int Count100;
        [Column("n50")] public int Count50;
        [Column("ngeki")] public int CountGeki;
        [Column("nkatu")] public int CountKatu;
        [Column("nmiss")] public int CountMiss;
        [Column("play_time")] public DateTime Time; 
        [Column("mode")] public int PlayMode;
        [Column("acc")] public double Accuracy;
        [Column("pp")] public float Performance;
    }
}