using System;

namespace FrameworkQ.ConsularServices.Services;

public class Token
{
    [Column("token_id")]
    public string TokenId { get; set; }

    [Column("generated_at")]
    public DateTime GeneratedAt { get; set; }

    [Column("appointment_at")]
    public DateTime AppointmentAt { get; set; }

    [Column("completed_at")]
    public DateTime CompletedAt { get; set; }

    [Column("description")]
    public string Description { get; set; }

    [Column("comment")]
    public string Comment { get; set; }

    [Column("attachments_received")]
    public string[] AttachmentsRecieved { get; set; }

    [Column("mobile_no")]
    public string MobileNo { get; set; }

    [Column("email")]
    public string Email { get; set; }

    [Column("service_type")]
    public int ServiceType { get; set; }
}