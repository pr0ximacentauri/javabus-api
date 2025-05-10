using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace javabus_api.Models
{
    [Table("users")]
    public class User
    {
        [Column("id_user")]
        [JsonIgnore]
        public int Id { get; set; }
        [Column("username")]
        public string Username { get; set; }
        [Column("full_name")]
        public string FullName { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("password")]
        public string Password { get; set; }

        [Column("fk_role")]
        [JsonIgnore]
        public int RoleId { get; set; } = 2;
        [JsonIgnore]
        public Role? Role { get; set; }
    }
}
