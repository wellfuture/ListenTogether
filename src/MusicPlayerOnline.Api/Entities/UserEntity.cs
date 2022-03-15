﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MusicPlayerOnline.Api.Entities
{
    [Table("UserBase")]
    public class UserEntity
    {
        [Key]
        public string Username { get; set; } = null!;
        public string Nickname { get; set; } = null!;
        public string Avatar { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool IsEnable { get; set; }


        [JsonIgnore]
        public List<RefreshTokenEntity> RefreshTokens { get; set; } = null!;
    }
}
