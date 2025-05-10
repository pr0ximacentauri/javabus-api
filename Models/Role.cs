using System.ComponentModel.DataAnnotations.Schema;

namespace javabus_api.Models
{
    [Table("roles")]
    public class Role
    {
        [Column("id_role")]
        public int Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
    }
}
