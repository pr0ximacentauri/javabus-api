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
    }
}
