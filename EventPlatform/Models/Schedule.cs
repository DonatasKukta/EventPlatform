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
    }
}
