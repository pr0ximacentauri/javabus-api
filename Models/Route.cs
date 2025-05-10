using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace javabus_api.Models
{
    [Table("routes")]
    public class Route
    {
        [Column("id_route")]
        public int Id { get; set; }
        [Column("origin_city_id")]
        public int OriginCityId { get; set; }
        [Column("destination_city_id")]
        public int DestinationCityId { get; set; }
        [Column("estimated_duration")]
        public TimeSpan? EstimatedDuration { get; set; }

        [JsonIgnore]
        public City? OriginCity { get; set; }
        [JsonIgnore]
        public City? DestinationCity { get; set; }
    }
}
