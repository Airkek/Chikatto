using System.ComponentModel.DataAnnotations.Schema;
using Chikatto.Constants;

namespace Chikatto.Database.Models
{
    public class DbChannel
    {
        [Column("name")] public string Name { get; set; }
        [Column("topic")] public string Topic { get; set; }
        [Column("read_priv")] public Privileges ReadPrivileges { get; set; }
        [Column("write_priv")] public Privileges WritePrivileges { get; set; }
        [Column("auto_join")] public bool AutoJoin { get; set; }
    }
}