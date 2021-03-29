using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Chikatto.Constants;

namespace Chikatto.Database.Models
{
    [Table("channels")]
    public class DbChannel
    {
        [Key]
        [Required]
        [Column("name")]
        public string Name { get; set; }

        [Required] [Column("topic")] public string Topic { get; set; }
        [Required] [Column("read_priv")] public Privileges ReadPrivileges { get; set; }
        [Required] [Column("write_priv")] public Privileges WritePrivileges { get; set; }
        [Required] [Column("auto_join")] public bool AutoJoin { get; set; }
    }
}