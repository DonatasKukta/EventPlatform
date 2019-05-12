using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;

namespace EventPlatform.Models
{
    public class User
    {
        [Key] public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public UserType Type { get; set; }
        public bool isCommentingRight { get; set; }
    }

    public enum UserType
    {
        admin,
        blocked,
        participant,
        organizer
    }

}
