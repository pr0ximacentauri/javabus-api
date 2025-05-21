using System.ComponentModel.DataAnnotations.Schema;

namespace javabus_api.Models
{
    [Table("bus_seats")]
    public class BusSeat
    {
        [Column("id_seat")]
        public int Id { get; set; }
        [Column("seat_number")]
        public string SeatNumber { get; set; }
        [Column("bus_id")]
        public int BusId { get; set; }
    }
}
