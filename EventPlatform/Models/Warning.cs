using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;

namespace EventPlatform.Models
{
    public class Warning
    {
        [Key] public int Id { get; set; }
        public string WarningSource { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }

        public int From_id { get; set; }
        public int To_id { get; set; }
    }
}
