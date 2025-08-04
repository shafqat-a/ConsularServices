using System.ComponentModel.DataAnnotations.Schema;

namespace FrameworkQ.ConsularServices.Services;

public class Station
{
    [Column("station_id")]
    public long QueueId { get; set; }

    [Column("station_name")]
    public string StationName { get; set; }

    [Column("status")]
    public int Status { get; set; }
}