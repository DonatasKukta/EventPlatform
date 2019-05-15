using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;

namespace EventPlatform.Models
{
    public class Tag
    {
        [Key] public int Id { get; set; }
        public string Name { get; set; }
        public int Weight { get; set; }
        
        public int User_id { get; set; }
        public int Event_id { get; set; }
    }
}
