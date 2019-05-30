using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace EventPlatform.Models
{
    public class Schedule
    {
        [Key] public int Id { get; set; }
        public int Event_id { get; set; }
        public int User_id { get; set; }

        public static Tuple<string, bool> Insert(int eventId, int userId)
        {
            using (var db = new Models.ModelContext())
            {
                var currEvent = db.Events.Where(e => e.Id.Equals(eventId));
                var currUser = db.Users.Where(u => u.Id.Equals(userId));

                // If any of the elements are not present in db
                if (!(currEvent.Any() && currUser.Any()))
                    return new Tuple<string, bool>("Atleiskite, įvyko klaida", false);

                var schedule = db.Schedules.Where(s => (s.Event_id == eventId) && (s.User_id == userId));
                if (schedule.Any())
                {
                    return new Tuple<string, bool>("Šis renginys jau pridėtas į Jūsų tvarkaraštį", false);
                }
                else
                {
                    var newSchedule = new Models.Schedule();
                    newSchedule.Event_id = eventId;
                    newSchedule.User_id = userId;
                    db.Schedules.Add(newSchedule);
                    db.SaveChanges();
                    return new Tuple<string, bool>("Renginys sėkmingai pridėtas į Jūsų tvarkaraštį.", true);
                }
            }
        }

        public static List<Schedule> Select(int id)
        {
            using (var db = new ModelContext())
            {
                var currrentUser = db.Users.FirstOrDefault(u => u.Id == id);

                if (currrentUser == null)
                {
                    return new List<Schedule>();
                }

                var scheduleInfo = db.Schedules.Where(s => s.User_id == id).ToList();

                return scheduleInfo;
            }
        }

        public static List<Event> SelectSheduleEvents(List<int> eventsIds)
        {
            var result = new List<Event>();

            using (var db = new ModelContext())
            {
                foreach (var eventId in eventsIds)
                {
                    var selectedEvent = db.Events.FirstOrDefault(e => e.Id == eventId);
                    if (selectedEvent != null && selectedEvent.Date.Month == DateTime.Now.Month)
                    {
                        result.Add(selectedEvent);
                    }
                }
            }

            return result;
        }

        public static void Delete(int eventId, int userId)
        {
            using (var db = new ModelContext())
            {
                var selectedSchedule =
                    db.Schedules.FirstOrDefault(x => x.Event_id == eventId && x.User_id == userId);

                if (selectedSchedule != null)
                {
                    db.Schedules.Remove(selectedSchedule);
                    db.SaveChanges();
                }
            }
        }
    }
}
