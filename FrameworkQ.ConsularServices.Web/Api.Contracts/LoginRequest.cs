namespace FrameworkQ.ConsularServices.Web.Api.Contracts;

public class LoginRequest
{
    public string Email { get; set; }
    public string PasswordHash { get; set; }
}