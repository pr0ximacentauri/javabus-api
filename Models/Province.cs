using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace javabus_api.Models
{
    [Table("provinces")]
    public class Province
    {
        [Column("id_province")]
        public int Id { get; set; }
        [Column("name")]
        public string Name { get; set; }   
    }
}
