﻿using System.ComponentModel.DataAnnotations;

namespace MusicPlayerOnline.Model.Api.Request
{
    public class UserRequest
    {
        [Required]
        public string Username { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}
