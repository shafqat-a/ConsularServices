namespace FrameworkQ.ConsularServices.Services;


public class ServiceInfo
{
    [Column("service_id")]
    public int ServiceId { get; set; }

    [Column("service_name")]
    public string ServiceName { get; set; }

    [Column("service_description")]
    public string ServiceDescription { get; set; }

    [Column("usual_service_days")]
    public int UsualServiceDays { get; set; }

    [Column("service_fee")]
    public double ServiceFee { get; set; }
}