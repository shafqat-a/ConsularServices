using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FrameworkQ.ConsularServices.Services;

[EntityMeta( Verb = "station", PKs = new[] { "StationId" })]
public class Station
{
    public enum  QueueStatus : int
    {
            Empty = 0,
            Away = 1,
            Waiting= 2,
            InProgress= 3,
            Completed= 4
    }

    [PropertyMeta(IsVisible = false)]
    [Key]
    [Column("station_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long StationId { get; set; }

    [Column("station_name")]
    public required string StationName { get; set; }

    [Column("queue_status")]
    public QueueStatus Status { get; set; }
}