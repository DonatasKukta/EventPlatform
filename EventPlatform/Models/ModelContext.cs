using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
//using System.ComponentModel.DataAnnotations;
//using System.Data.Entity;

namespace EventPlatform.Models
{
    public class ModelContext : DbContext
    {

        public DbSet<Event> Events { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=./EventDB.db");
        }
    }

    

    
}
