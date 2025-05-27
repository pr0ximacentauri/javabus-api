using System.ComponentModel.DataAnnotations.Schema;

namespace javabus_api.Models
{
    [Table("tickets")]
    public class Ticket
    {
        [Column("id_ticket")]
        public int Id { get; set; }
        [Column("booking_id")]
        public int BookingId { get; set; }
        [Column("seat_id")]
        public int SeatId { get; set; }
        [Column("qr_code_url")]
        public string QrCodeUrl { get; set; }
        [Column("departure_time")]
        public DateTime DepartureTime { get; set; }
        [Column("origin_city")]
        public string OriginCity { get; set; }
        [Column("destination_city")]
        public string DestinationCity { get; set; }
        [Column("bus_name")]
        public string BusName { get; set; }
        [Column("bus_class")]
        public string BusClass { get; set; }
        [Column("ticket_price")]
        public int TicketPrice { get; set; }
        [Column("ticket_status")]
        public string TicketStatus { get; set; }
    }
}
