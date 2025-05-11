using javabus_api.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace javabus_api.Contexts
{
    public class ApplicationDBContext : DbContext
    {
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Bus> Buses {  get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Payment> Payment { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<Models.Route> Routes { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<User> Users { get; set; }

        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) 
            : base(options)
        {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>()
                .HasOne(c => c.Province)
                .WithMany()
                .HasForeignKey(c => c.ProvinceId)
                .OnDelete(DeleteBehavior.Restrict);
            //modelBuilder.Entity<City>()
            //    .Property(c => c.ProvinceId)
            //    .HasColumnName("fk_province");

            modelBuilder.Entity<Models.Route>()
                .HasOne(r => r.OriginCity)
                .WithMany()
                .HasForeignKey(r => r.OriginCityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Models.Route>()
                .HasOne(r => r.DestinationCity)
                .WithMany()
                .HasForeignKey(r => r.DestinationCityId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
