using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace javabus_api.Models
{
    [Table("buses")]
    public class Bus
    {
        [Column("id_bus")]
        public int Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("class")]
        public string BusClass { get; set; }
        [Column("total_seat")]
        public int TotalSeat { get; set; }
    }
}
