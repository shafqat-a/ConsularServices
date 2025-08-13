using System.ComponentModel.DataAnnotations.Schema;

namespace FrameworkQ.ConsularServices.Services;

[ActionVerb( Verb = "service", PKs = new[] { "ServiceId" })]
public class Service
{
    [System.ComponentModel.DataAnnotations.Key]
    [Column("service_id")]
    public long ServiceId { get; set; }

    [Column("service_name")]
    public string ServiceName { get; set; }

    [Column("service_description")]
    public string ServiceDescription { get; set; }

    [Column("usual_service_days")]
    public int UsualServiceDays { get; set; }

    [Column("service_fee")]
    public double ServiceFee { get; set; }
}