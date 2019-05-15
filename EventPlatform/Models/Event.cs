using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace EventPlatform.Models
{
    public class Event
    {
        [Key] public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Annotation { get; set; }
        public DateTime Date { get; set; }
        public byte[] Image { get; set; }
        public string Place { get; set; }
        public float Price { get; set; }
        public EventEnum State { get; set; }

        public int Duration_id { get; set; }

        public static List<Event> GetEventList()
        {
            using (var db = new Models.ModelContext())
            {
                return db.Events.ToList();
            }
        }
    }


    public enum EventEnum
    {
        cancelled,
        upcoming,
        ongoing,
        ended
    }

    

}
