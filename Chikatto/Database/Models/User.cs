using System.ComponentModel.DataAnnotations.Schema;
using Chikatto.Constants;

namespace Chikatto.Database.Models
{
    public class User
    {
        [Column("id")] public int Id;
        [Column("name")] public string Name;
        [Column("safe_name")] public string SafeName;
        [Column("pw_bcrypt")] public string Password;
        [Column("creation_time")] public int JoinTimestamp;
        [Column("email")] public string Email;
        [Column("priv")] public Privileges Privileges;
        [Column("country")] public string Country;
    }
}