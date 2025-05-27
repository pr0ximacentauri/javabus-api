using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace javabus_api.Models
{
    [Table("schedules")]
    public class Schedule
    {
        [Column("id_schedule")]
        public int Id { get; set; }
        [Column("departure_time")]
        public DateTime DepartureTime { get; set; }
        [Column("ticket_price")]
        public int TicketPrice { get; set; }
        [Column("bus_id")]
        public int BusId { get; set; }
        [Column("route_id")]
        public int RouteId { get; set; }


        [JsonIgnore]
        public Bus? Bus { get; set; }
        [JsonIgnore]
        public Models.BusRoute? Route { get; set; }
    }
}
