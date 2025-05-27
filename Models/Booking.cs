using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace javabus_api.Models
{
    [Table("bookings")]
    public class Booking
    {
        [Column("id_booking")]
        public int Id { get; set; }
        [Column("status_booking")]
        public string Status { get; set; }
        [Column("user_id")]
        public int UserId { get; set; }
        [Column("schedule_id")]
        public int ScheduleId {  get; set; }

        [JsonIgnore]
        public User? User { get; set; }
        [JsonIgnore]
        public Schedule? Schedule { get; set; }

    }
}
