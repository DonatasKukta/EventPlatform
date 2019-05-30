using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace EventPlatform.Models
{
    public class Tag
    {
        [Key] public int Id { get; set; }
        public string Name { get; set; }
        public int Weight { get; set; }
        
        public int User_id { get; set; }
        public int Event_id { get; set; }

        public static List<Tag> SelectList()
        {
            using (var db = new ModelContext())
            {
                return db.Tags.ToList();
            }
        }

        public static void Insert(Tag newTag)
        {
            using (var db = new ModelContext())
            {
                db.Add(newTag);
                db.SaveChanges();
            }
        }

        public static void Remove(Tag tagToDelete)
        {
            using (var db = new ModelContext())
            {
                db.Remove(tagToDelete);
                db.SaveChanges();
            }
        }

        public static void Update(Tag tagToUpdate)
        {
            using (var db = new ModelContext())
            {
                db.Update(tagToUpdate);
                db.SaveChanges();
            }
        }

        public string GetNameValue()
        {
            switch (Name)
            {
                case "amuzikinis":
                    return "Muzikinis";
                case "asportinis":
                    return "Sportinis";
                case "apoilsinis":
                    return "Poilsinis";
                case "apazintinis":
                    return "Pazintinis";
                case "bgamtoje":
                    return "Gamtoje";
                case "bklube":
                    return "Klube";
                case "boficialiai":
                    return "Oficialiai";
                case "bkeliaujant":
                    return "Keliaujant";
                case "cjaunimas":
                    return "Jaunimas";
                case "cvaikai":
                    return "Vaikai";
                case "cvidAmzius":
                    return "Vidutinio";
                case "csenimas":
                    return "senimas";
                default:
                    return "-";

            }
        }
    }
}
