using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FrameworkQ.ConsularServices.Web.Api.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using FrameworkQ.ConsularServices;

namespace FrameworkQ.ConsularServices.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class ApiController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    
    public ApiController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    // This action will respond to GET requests at /api/SampleData/greeting
    [HttpGet("greeting")]
    public IActionResult GetGreeting()
    {
        var data = new
        {
            Message = "Hello from the API!",
            Timestamp = DateTime.UtcNow
        };

        return Ok(data); // Returns a 200 OK response with the JSON data
    }

    private string Hash256SHA(string inputstring)
    {
        using (var sha256 = System.Security.Cryptography.SHA256.Create())
        {
            // Convert the input string to a byte array and compute the hash
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(inputstring);
            byte[] hash = sha256.ComputeHash(bytes);

            // Convert byte array to a string
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                stringBuilder.Append(hash[i].ToString("x2"));
            }

            return stringBuilder.ToString();
        }
    }
    
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request,
        [FromServices] IOptions<JwtOptions> jwtOpts)
    {
        // 1. Get user by email from database
        var user = _userRepository.GetUserByEmail(request.Email);
        if (user == null)
            return Unauthorized(new { Message = "Invalid username or password." });

        // 2. Hash the provided password and compare with stored hash
        var hashedPassword = request.PasswordHash;
        if (user.PasswordHash != hashedPassword)
            return Unauthorized(new { Message = "Invalid username or password." });

        // 3. Create claims
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, request.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, request.Email)
            // add roles, etc. here
        };

        // 4. Read options from DI
        var opts = jwtOpts.Value;

        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(opts.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer:  opts.Issuer,
            audience:opts.Audience,
            claims:   claims,
            expires:  DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        // 5. Return token
        return Ok(new
        {
            Message = "Login successful!",
            Token   = new JwtSecurityTokenHandler().WriteToken(token)
        });
    }
}