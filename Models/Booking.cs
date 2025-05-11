using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace javabus_api.Models
{
    [Table("bookings")]
    public class Booking
    {
        [Column("id_booking")]
        public int Id { get; set; }
        [Column("status")]
        public string Status { get; set; }
        [Column("fk_user")]
        public int UserId { get; set; }
        [Column("fk_schedule")]
        public int ScheduleId {  get; set; }

    }
}
