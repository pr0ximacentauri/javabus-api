using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace javabus_api.Models
{
    [Table("seat_bookings")]
    public class SeatBooking
    {
        [Column("id_seat_booking")]
        public int Id { get; set; }
        [Column("booking_id")]
        public int BookingId { get; set; }
        [Column("schedule_id")]
        public int ScheduleId { get; set; }
        [Column("seat_id")]
        public int SeatId { get; set; }

        [JsonIgnore]
        public Booking Booking { get; set; }
        [JsonIgnore]
        public Schedule Schedule { get; set; }
        [JsonIgnore]
        public BusSeat Seat { get; set; }
    }
}
