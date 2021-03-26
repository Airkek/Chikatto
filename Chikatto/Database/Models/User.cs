﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Chikatto.Bancho;
using Chikatto.Constants;

namespace Chikatto.Database.Models
{
    [Table("users")]
    public class User
    {
        [Required]
        [Column("id")] 
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("name")] [Required] public string Name { get; set; }
        [Column("safe_name")] [Required] public string SafeName { get; set; }
        [Column("pw_bcrypt")] [Required] public string Password { get; set; }
        [Column("creation_time")] [Required] public int JoinTimestamp { get; set; }
        [Column("email")] [Required] public string Email { get; set; }
        [Column("priv")] [Required] public Privileges Privileges { get; set; }
        [Column("country")] [Required] public string Country { get; set; }

        public long LastPong = 0;
        public List<Packet> WaitingPackets = new ();
        public override string ToString() => $"<{Name} ({Id})>";
    }
}