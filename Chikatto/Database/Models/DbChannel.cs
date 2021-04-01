using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Chikatto.Constants;

namespace Chikatto.Database.Models
{
    public class DbChannel
    {
        [Column("name")] public string Name;
        [Column("topic")] public string Topic;
        [Column("read_priv")] public Privileges ReadPrivileges;
        [Column("write_priv")] public Privileges WritePrivileges;
        [Column("auto_join")] public bool AutoJoin;
    }
}