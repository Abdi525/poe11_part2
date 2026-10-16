using poe11.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Reflection;
using System.Reflection.Emit;

namespace poe11.RaceDayDatabase
{
    public class RaceDayDatabaseContext : DbContext
    {

        public RaceDayDatabaseContext (): base()
        { }

            public DbSet<Users> Users { get; set; }
            public DbSet<Event> Events { get; set; }
            public DbSet<EventTypes> EventTypes { get; set; }
            public DbSet<RouteMap> RouteMaps { get; set; }
            public DbSet<RaceEntrance> RaceEntrances { get; set; }
            public DbSet<Models.Results> Results { get; set; }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Users>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<RaceEntrance>().HasIndex(r => r.ChipNumber).IsUnique();
            modelBuilder.Entity<Models.Results>().HasIndex(r => r.EntranceId).IsUnique();
            modelBuilder.Entity<RouteMap>().HasIndex(r => r.EventTypeCode).IsUnique();
        }
    }
}
