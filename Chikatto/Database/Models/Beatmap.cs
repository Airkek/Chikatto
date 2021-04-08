using System.ComponentModel.DataAnnotations.Schema;
using Chikatto.Enums;

namespace Chikatto.Database.Models
{
    public class Beatmap
    {
        [Column("id")] public int Id { get; set; }
        [Column("md5")] public string Checksum { get; set; }
        [Column("status")] public RankedStatus Status { get; set; }
        [Column("set_id")] public int SetId { get; set; }
        [Column("artist")] public string Artist { get; set; }
        [Column("title")] public string Title { get; set; }
        [Column("version")] public string Version { get; set; }
        [Column("creator")] public string Creator { get; set; }
        [Column("plays")] public int PlayCount { get; set; }
        [Column("passes")] public int PassCount { get; set; }
        [Column("frozen")] public bool Frozen { get; set; }
        [Column("bpm")] public float BPM { get; set; }
        [Column("cs")] public float CS { get; set; }
        [Column("ar")] public float AR { get; set; }
        [Column("od")] public float OD { get; set; }
        [Column("hp")] public float HP { get; set; }
        [Column("diff")] public float SR { get; set; }
    }
}