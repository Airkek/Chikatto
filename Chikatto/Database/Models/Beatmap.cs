using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Chikatto.Constants;

namespace Chikatto.Database.Models
{
    public class Beatmap
    {
        [Column("id")] public int Id;
        [Column("md5")] public string Checksum;
        [Column("status")] public RankedStatus Status;
        [Column("set_id")] public int SetId;
        [Column("artist")] public string Artist;
        [Column("title")] public string Title;
        [Column("version")] public string Version;
        [Column("creator")] public string Creator;
        [Column("plays")] public int PlayCount;
        [Column("passes")] public int PassCount;
        [Column("frozen")] public bool Frozen;
        [Column("bpm")] public float BPM;
        [Column("cs")] public float CS;
        [Column("ar")] public float AR;
        [Column("od")] public float OD;
        [Column("hp")] public float HP;
        [Column("diff")] public float SR;
    }
}