using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;

namespace EventPlatform.Models
{
    public class Rating
    {
        [Key]public int Id { get; set; }
        public int Score { get; set; }

        public Event Event { get; set; }
        public User User { get; set; }
    }
}
