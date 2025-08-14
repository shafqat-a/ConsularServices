namespace FrameworkQ.ConsularServices.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ServiceInstance
{
    [Key]
    [Column("service_instance_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long ServiceInstanceId { get; set; }
    
    [Column("service_info_id")]
    public long ServiceInfoId { get; set; }
    
    [Column("payment_made_at")]
    public DateTime PaymentMadeAt { get; set; }
    
    [Column("delivery_date")]
    public DateTime DeliveryDate { get; set; }
    
    [Column("delivered_at")]
    public DateTime DeliveredAt { get; set; }
    
    [Column("note")]
    public string? Note { get; set; }

    [Column("attachments_received")]
    public string[]? AttachmentsRecieved { get; set; }
    
    [Column("token_id")]
    public required string TokenId { get; set; }

}