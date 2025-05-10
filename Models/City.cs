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
        [Column("fk_province")]
        public int ProvinceId { get; set; }
        [JsonIgnore]
        //[ForeignKey("ProvinceId")]
        public Province? Province { get; set; }
    }
}
