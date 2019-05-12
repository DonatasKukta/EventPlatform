using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;

namespace EventPlatform.Models
{
    public class Promotion
    {
        [Key] public int Id { get; set; }
        public OrderState State { get; set; }
        public DateTime Date { get; set; }
        public byte[] Image { get; set; }
        public string Annotation { get; set; }

        public User User { get; set; }
        public Event Event { get; set; }
    }

    public enum OrderState
    {
        approved,
        waitingApproval,
        rejected
    }
}
