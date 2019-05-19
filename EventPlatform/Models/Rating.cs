using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace EventPlatform.Models
{
    public class Rating
    {
        [Key]public int Id { get; set; }
        public int Score { get; set; }

        public int Event_id { get; set; }
        public int User_id { get; set; }

        public static Rating Select(int eventId, int userId)
        {
            using (var db = new Models.ModelContext())
            {
                return db.Ratings.Where(s => (s.Event_id == eventId) && (s.User_id == userId)).FirstOrDefault();
            }
        }

        public static double GetRating(int eventId)
        {
            using (var db = new Models.ModelContext())
            {
                int avgRating = 0;
                var ratings = db.Ratings.Where(s => s.Event_id == eventId).ToList();
                foreach (var rating in ratings)
                {
                    avgRating += rating.Score;
                }

                return (double)avgRating / ratings.Count();
            }
        }

        public static Tuple<string, bool> Insert(int eventId, int userId, int ratingValue)
        {
            using (var db = new Models.ModelContext())
            {
                var currEvent = db.Events.Where(e => e.Id.Equals(eventId));
                var currUser = db.Users.Where(u => u.Id.Equals(userId));

                // If any of the elements are not present in db
                if (!(currEvent.Any() && currUser.Any()))
                    return new Tuple<string, bool>("Atleiskite, įvyko klaida", false);

                var rating = db.Ratings.Where(s => (s.Event_id == eventId) && (s.User_id == userId));
                if (!Validate(ratingValue))
                {
                    return new Tuple<string, bool>("Jūsų įvertinimas yra netinkamas", false);
                }
                else if (rating.Any())
                {
                    foreach (var score in rating)
                    {
                        score.Score = ratingValue;
                    }
                    db.SaveChanges();
                    return new Tuple<string, bool>("Jūsų įvertinimas pakeistas", false);
                }
                else
                {
                    var newRating = new Models.Rating();
                    newRating.Event_id = eventId;
                    newRating.User_id = userId;
                    newRating.Score = ratingValue;
                    db.Ratings.Add(newRating);
                    db.SaveChanges();
                    return new Tuple<string, bool>("Renginys sėkmingai įvertintas.", true);
                }
            }
        }

        private static bool Validate(int rating)
        {
            if (rating <= 5 && rating >= 1)
                return true;
            else return false;
        }

    }

    
}
