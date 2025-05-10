using System.ComponentModel.DataAnnotations.Schema;

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
        [Column("plate_number")]
        public string PlateNumber { get; set; }
        [Column("total_seat")]
        public int TotalSeat { get; set; }
    }
}
