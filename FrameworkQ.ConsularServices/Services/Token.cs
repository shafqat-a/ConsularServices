using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FrameworkQ.ConsularServices.Services;

public class Token
{
    [System.ComponentModel.DataAnnotations.Key]
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

    [Column("mobile_no")]
    public string MobileNo { get; set; }

    [Column("email")]
    public string Email { get; set; }

    [Column("service_type")]
    public long[] ServiceType { get; set; }
    
    [Column("passport_no")]
    public string PassportNo { get; set; }
    
    [Column("nid_no")]
    public string NationalIdNo { get; set; }

}