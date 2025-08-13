using System.ComponentModel.DataAnnotations.Schema;

namespace FrameworkQ.ConsularServices.Services;

[ActionVerb( Verb = "station", PKs = new[] { "StationId" })]
public class Station
{
    [System.ComponentModel.DataAnnotations.Key]
    [Column("station_id")]
    public long StationId { get; set; }

    [Column("station_name")]
    public string StationName { get; set; }

    [Column("queue_status")]
    public int Status { get; set; }
}