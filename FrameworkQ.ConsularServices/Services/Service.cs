using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FrameworkQ.ConsularServices.Services;

[EntityMeta(Verb = "service", PKs = new[] { "ServiceId" })]
public class Service
{
    [PropertyMeta(IsVisible = false)]
    [Key]
    [Column("service_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long ServiceId { get; set; }

    [Column("service_name")]
    [Required]
    public required string ServiceName { get; set; }

    [Column("service_description")]
    public required string ServiceDescription { get; set; }

    [Column("usual_service_days")]
    public int UsualServiceDays { get; set; }

    [Column("service_fee")]
    public double ServiceFee { get; set; }
}