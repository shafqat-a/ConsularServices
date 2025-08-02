namespace FrameworkQ.ConsularServices.Web;

public sealed class JwtOptions
{
    public string Key     { get; init; } = default!;
    public string Issuer  { get; init; } = default!;
    public string Audience{ get; init; } = default!;
}