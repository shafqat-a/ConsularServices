using System.ComponentModel.DataAnnotations.Schema;

namespace FrameworkQ.ConsularServices.Services;

public class Queue
{
    [Column("queue_id")]
    public long QueueId { get; set; }

    [Column("queue_name")]
    public string QueueName { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("queue_status")]
    public int QueueStatus { get; set; }
}