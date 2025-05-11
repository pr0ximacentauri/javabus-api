using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace javabus_api.Models
{
    [Table("payments")]
    public class Payment
    {
        [Column("id_payment")]
        public int Id { get; set; }
        [Column("fk_booking")]
        public int BookingId { get; set; }
        [Column("id_order")]
        public int OrderId { get; set; }
        [Column("gross_amount")]
        public int GrossAmount { get; set; }
        [Column("payment_type")]
        public string PaymentType { get; set; }
        [Column("payment_url")]
        public string PaymentUrl { get; set; }
        [Column("transcation_status")]
        public string TransactionStatus { get; set; }
        [Column("transaction_time")]
        public DateTime TransactionTime { get; set; }
    }
}
