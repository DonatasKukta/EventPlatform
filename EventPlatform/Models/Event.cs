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
        public int User_id { get; set; }

        public static Event Select(int eventId)
        {
            using (var db = new Models.ModelContext())
            {
                return db.Events.Where(e => e.Id.Equals(eventId)).FirstOrDefault();
            }
        }

        public static List<Event> SelectList(string option)
        {
            using (var db = new Models.ModelContext())
            {
                if (option.Equals(""))
                {
                    return db.Events.ToList();
                }
                else if(option.Equals("ended"))
                {
                    var currDate = DateTime.Today;
                    return db.Events.Where( e => e.Date < currDate).ToList();
                }
                else if (option.Equals("upcoming"))
                {
                    var currDate = DateTime.Today;
                    return db.Events.Where(e => e.Date >= currDate).ToList();
                }
                else
                {
                    return new List<Event>();
                }
            }
        }
        public static string getType(EventEnum type)
        {
            if (type == EventEnum.cancelled)
                return "Atšauktas";
            else if (type == EventEnum.upcoming)
                return "Vyksiantis";
            else if (type == EventEnum.ongoing)
                return "Vykstantis";
            else if (type == EventEnum.ended)
                return "Pasibaigęs";
            else
                return string.Empty;
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
