using System.ComponentModel.DataAnnotations.Schema;

namespace javabus_api.Models
{
    [Table("provinces")]
    public class Province
    {
        [Column("id_province")]
        public int Id { get; set; }
        [Column("name")]
        public string Name { get; set; }    
        //public ICollection<City> Cities { get; set; }
    }
}
