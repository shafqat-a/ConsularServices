namespace FrameworkQ.ConsularServices.Services;

public class Token
{
    public string TokenId { get; set; } // token format YMDDnnnn N - 2025 = 1, 2026 =2
    public DateTime GeneratedAt { get; set; }
    public DateTime AppointmentAt { get; set; }
    public DateTime CompletedAt { get; set; }
    public string Description { get; set; }
    public string Comment { get; set; }
    public string[] AttachmentsRecieved { get; set; }
    public string MobileNo { get; set; }
    public string Email { get; set; }
    public int ServiceType { get; set; }
}