namespace FrameworkQ.ConsularServices.Web.Api.Contracts;

public class LoginResponse
{
    public bool IsSuccessful { get; set; }
    public string FailureReason { get; set; }
    public string NameOfUser { get; set; }
    public string SessionToken { get; set; }
}