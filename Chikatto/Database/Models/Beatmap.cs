using System.ComponentModel.DataAnnotations.Schema;
using Chikatto.Enums;

namespace Chikatto.Database.Models
{
    public class Beatmap
    {
        [Column("id")] public int Id { get; set; }
        [Column("rankedby")] public string RankedBy { get; set; }
        [Column("beatmap_md5")] public string Checksum { get; set; }
        [Column("beatmap_id")] public int MapId { get; set; }
        [Column("beatmapset_id")] public int SetId { get; set; }
        [Column("song_name")] public string Name { get; set; }
        [Column("file_name")] public string FileName { get; set; }
        [Column("ar")] public float AR { get; set; }
        [Column("od")] public float OD { get; set; }
        [Column("mode")] public GameMode Mode { get; set; }
        [Column("rating")] public int Rating { get; set; }
        [Column("difficulty_std")] public float DiffSTD { get; set; }
        [Column("difficulty_taiko")] public float DiffTaiko { get; set; }
        [Column("difficulty_ctb")] public float DiffCTB { get; set; }
        [Column("difficulty_mania")] public float DiffMania { get; set; }
        [Column("max_combo")] public int MaxCombo { get; set; }
        [Column("hit_length")] public int HitLength { get; set; }
        [Column("bpm")] public int BPM { get; set; }
        [Column("playcount")] public int Playcount { get; set; }
        [Column("passcount")] public int Passcount { get; set; }
        [Column("ranked")] public RankedStatus Status { get; set; }
        [Column("latest_update")] public int LatestUpdate { get; set; }
        [Column("ranked_status_freezed")] public bool Frozen { get; set; }
        [Column("disable_pp")] public int DisablePP { get; set; }
     }
}