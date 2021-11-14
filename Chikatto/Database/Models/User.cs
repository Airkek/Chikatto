using System.ComponentModel.DataAnnotations.Schema;
using Chikatto.Enums;

namespace Chikatto.Database.Models
{
    public class User
    {
        [Column("id")] public int Id { get; set; }
        [Column("username")] public string Name { get; set; }
        [Column("username_safe")] public string SafeName { get; set; }
        [Column("password_md5")] public string Password { get; set; }
        [Column("register_datetime")] public int JoinTimestamp { get; set; }
        [Column("email")] public string Email { get; set; }
        [Column("privileges")] public Privileges Privileges { get; set; }
        [Column("silence_end")] public int SilenceEnd { get; set; }
        
        //TODO: add all rows
    }
}