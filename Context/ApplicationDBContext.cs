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
        public DbSet<BusSeat> BusSeats { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<BusRoute> Routes { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<SeatBooking> SeatBookings { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

        public DbSet<User> Users { get; set; }

        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) 
            : base(options)
        {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Schedule)
                .WithMany()
                .HasForeignKey(b => b.ScheduleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BusRoute>()
                .HasOne(r => r.OriginCity)
                .WithMany()
                .HasForeignKey(r => r.OriginCityId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<BusRoute>()
                .HasOne(r => r.DestinationCity)
                .WithMany()
                .HasForeignKey(r => r.DestinationCityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BusSeat>()
                .HasOne(bs => bs.Bus)
                .WithMany()
                .HasForeignKey(bs => bs.BusId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<City>()
                .HasOne(c => c.Province)
                .WithMany()
                .HasForeignKey(c => c.ProvinceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.Bus)
                .WithMany() 
                .HasForeignKey(s => s.BusId)
                .OnDelete(DeleteBehavior.Restrict); 
            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.Route)
                .WithMany()
                .HasForeignKey(s => s.RouteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SeatBooking>()
                .HasOne(sb => sb.Booking)
                .WithMany()
                .HasForeignKey(sb => sb.BookingId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SeatBooking>()
                .HasOne(sb => sb.Schedule)
                .WithMany()
                .HasForeignKey(sb => sb.ScheduleId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SeatBooking>()
                .HasOne(sb => sb.Seat)
                .WithMany()
                .HasForeignKey(sb => sb.SeatId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SeatBooking>()
                .HasIndex(sb => new { sb.ScheduleId, sb.SeatId })
                .IsUnique();

        }
    }
}
