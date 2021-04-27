using System.ComponentModel.DataAnnotations.Schema;

namespace Chikatto.Database.Models
{
    public class Friendships
    {
        [Column("id")] public int Id { get; set; }
        [Column("user1")] public int UserId { get; set; }
        [Column("user2")] public int FriendId { get; set; }
    }
}