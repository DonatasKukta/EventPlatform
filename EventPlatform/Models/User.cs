using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace EventPlatform.Models
{
    public class User
    {
        [Key] public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public UserType Type { get; set; }
        public bool isCommentingRight { get; set; }

        public static User getUser(string username)
        {
            using (var db = new Models.ModelContext())
            {
                return db.Users.Where(u => u.Username == username).FirstOrDefault();
            }
        }
        public static User getUser(int id)
        {
            using (var db = new Models.ModelContext())
            {
                return db.Users.Where(u => u.Id == id).FirstOrDefault();
            }
        }

        public static List<User> getUserList()
        {
            using (var db = new Models.ModelContext())
            {
                return db.Users.ToList();
            }
        }

        public static string getType(UserType type)
        {
            if (type == UserType.admin)
                return "admin";
            else if (type == UserType.organizer)
                return "organizer";
            else if (type == UserType.participant)
                return "participant";
            else if (type == UserType.blocked)
                return "blocked";
            else
                return string.Empty;
        }

        public static bool isNormalUser(UserType type)
        {
            if (type == UserType.admin)
                return true;
            else if (type == UserType.organizer)
                return true;
            else if (type == UserType.participant)
                return true;
            else if (type == UserType.blocked)
                return false;
            else
                return false;
        }

        public static List<Tag> GetUserInterests(int userId)
        {
            using (var db = new ModelContext())
            {
                var userTags = new List<Tag>();
                var allTags = db.Tags.ToList();
                foreach (var singleTag in allTags)
                {
                    if (singleTag.Weight != -1 && singleTag.User_id == userId)
                    {
                        userTags.Add(singleTag);
                    }   
                }
                return userTags;
            }
        }
    }

    public enum UserType
    {
        admin,
        blocked,
        participant,
        organizer
    }

}
