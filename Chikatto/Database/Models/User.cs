using System.ComponentModel.DataAnnotations.Schema;
using Chikatto.Enums;

namespace Chikatto.Database.Models
{
    public class User
    {
        [Column("id")] public int Id { get; set; }
        [Column("name")] public string Name { get; set; }
        [Column("safe_name")] public string SafeName { get; set; }
        [Column("pw_bcrypt")] public string Password { get; set; }
        [Column("creation_time")] public int JoinTimestamp { get; set; }
        [Column("email")] public string Email { get; set; }
        [Column("priv")] public Privileges Privileges { get; set; }
        [Column("country")] public string Country { get; set; }
    }
}