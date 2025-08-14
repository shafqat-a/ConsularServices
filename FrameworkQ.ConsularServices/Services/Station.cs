using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FrameworkQ.ConsularServices.Services;

[ActionVerb( Verb = "station", PKs = new[] { "StationId" })]
public class Station
{
    [MetaData(IsVisible =false)]
    [Key]
    [Column("station_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long StationId { get; set; }

    [Column("station_name")]
    public required string StationName { get; set; }

    [Column("queue_status")]
    public int Status { get; set; }
}