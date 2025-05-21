using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace javabus_api.Models
{
    [Table("cities")]
    public class City
    {
        [Column("id_city")]
        public int Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("province_id")]
        public int ProvinceId { get; set; }
        public Province? Province { get; set; }
    }
}
