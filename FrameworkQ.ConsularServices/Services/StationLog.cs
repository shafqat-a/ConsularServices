using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FrameworkQ.ConsularServices.Services
{
    [Table("station_log")]
    public class StationLog
    {
        [Key]
        [Column("log_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long LogId { get; set; }

        [Column("station_id")]
        public long StationId { get; set; }

        [Column("user_id")]
        public long UserId { get; set; }

        [Column("note")]
        public string? Note { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("start_time")]
        public DateTime? StartTime { get; set; }

        [Column("end_time")]
        public DateTime? EndTime { get; set; }
    }
}
