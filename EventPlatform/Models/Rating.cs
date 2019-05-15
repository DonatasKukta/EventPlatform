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

        public int Event_id { get; set; }
        public int User_id { get; set; }
    }
}
