using System.ComponentModel.DataAnnotations.Schema;
using Chikatto.Enums;

namespace Chikatto.Database.Models
{
    public class DbChannel
    {
        [Column("id")] public int Id { get; set; }
        [Column("name")] public string Name { get; set; }
        [Column("description")] public string Topic { get; set; }
        [Column("public_read")] public bool PublicRead { get; set; }
        [Column("public_write")] public bool PublicWrite { get; set; }
        [Column("status")] public int Status { get; set; }
        [Column("temp")] public bool Temp { get; set; }
        [Column("hidden")] public bool Hidden { get; set; }
    }
}