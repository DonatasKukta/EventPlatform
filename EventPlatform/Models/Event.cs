﻿using Microsoft.EntityFrameworkCore;
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

        public TimeSpan Duration { get; set; }
        public int User_id { get; set; }

        public static Event Select(int eventId)
        {
            using (var db = new Models.ModelContext())
            {
                return db.Events.Where(e => e.Id.Equals(eventId)).FirstOrDefault();
            }
        }
        public static void Insert(Event evnt)
        {
            using (var db = new ModelContext())
            {
                db.Add(evnt);
                db.SaveChanges();
            }
        }

        public static List<Event> SelectListOrganizer(string option, int option2)
        {
            using (var db = new Models.ModelContext())
            {
                if (option == null || option.Equals(""))
                {
                    return db.Events.Where(e => e.User_id == option2).ToList();
                }
                else if (option.Equals("ended"))
                {
                    var currDate = DateTime.Today;
                    return db.Events.Where(e => e.Date < currDate && e.User_id == option2).ToList();
                }
                else if (option.Equals("upcoming"))
                {
                    var currDate = DateTime.Today;
                    return db.Events.Where(e => e.Date >= currDate && e.User_id == option2).ToList();
                }
                else
                {
                    return new List<Event>();
                }
            }
        }

        public static List<Event> SelectList(string option)
        {
            using (var db = new Models.ModelContext())
            {
                if (option == null || option.Equals(""))
                {
                    return db.Events.ToList();
                }
                else if (option.Equals("ended"))
                {
                    var currDate = DateTime.Today;
                    return db.Events.Where(e => e.Date < currDate).ToList();
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
        public static void Delete(Event evnt)
        {
            using (var db = new ModelContext())
            {
                var rating = db.Ratings.Where(r => r.Event_id == evnt.Id);
                var schedule = db.Schedules.Where(s => s.Event_id == evnt.Id);
                var tags = db.Tags.Where(t => t.Event_id == evnt.Id);
                var promotions = db.Promotions.Where(p => p.Event_id == evnt.Id);
                db.Promotions.AttachRange(promotions);
                db.Promotions.RemoveRange(promotions);
                db.SaveChanges();
                db.Tags.AttachRange(tags);
                db.Tags.RemoveRange(tags);
                db.SaveChanges();
                db.Ratings.AttachRange(rating);
                db.Ratings.RemoveRange(rating);
                db.SaveChanges();
                db.Schedules.AttachRange(schedule);
                db.Schedules.RemoveRange(schedule);
                db.SaveChanges();
                db.Events.Attach(evnt);
                db.Events.Remove(evnt);

                db.SaveChanges();
            }

        }

        public static void Update(Event evnt)
        {
            using (var db = new ModelContext())
            {
                db.Update(evnt);
                db.SaveChanges();
            }
        }
        public static string Update(int eventId, int state)
        {
            using (var db = new Models.ModelContext())
            {
                if (0 <= state && state < 3)
                {
                    var currEvent = db.Events.FirstOrDefault(p => p.Id.Equals(eventId));
                    if (currEvent != null)
                    {
                        if (currEvent.State == (EventEnum)state)
                            return "Negalima pekeisti renginio būsenos į tokią pačia";
                        else
                        {
                            currEvent.State = (EventEnum)state;
                            db.Update(currEvent);
                            db.SaveChanges();
                            return "Užsakymo būsena sėkmingai atnaujinta";
                        }
                    }
                    else
                        return "Šis užsakymas nerastas";
                }
                else
                    return "Neteisinga užsakymo būsena";
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
