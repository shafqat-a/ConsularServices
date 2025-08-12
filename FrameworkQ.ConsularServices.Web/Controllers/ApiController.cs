using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FrameworkQ.ConsularServices.Web.Api.Contracts;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using FrameworkQ.ConsularServices.Services;

namespace FrameworkQ.ConsularServices.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class ApiController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public ApiController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
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



    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request,
        [FromServices] IOptions<JwtOptions> jwtOpts)
    {
        // 1. Get user by email from database
        var user = _serviceManager.GetUserByEmail(request.Email);
        if (user == null)
            return Unauthorized(new { Message = "Invalid username or password." });

        // 2. Hash the provided password and compare with stored hash
        var hashedPassword = request.PasswordHash;
        if (user.PasswordHash.ToUpper() != hashedPassword.ToUpper())
            return Unauthorized(new { Message = "Invalid username or password." });

        // 3. Create claims
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, request.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, request.Email)
            // add roles, etc. here
        };

        //var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

        // 4. Read options from DI
        var opts = jwtOpts.Value;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(opts.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: opts.Issuer,
            audience: opts.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        // 5. Write token to cookie instead of returning it
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        HttpContext.Response.Cookies.Append("jwt", tokenString, new CookieOptions
        {
            HttpOnly = true,             // Prevent access from JavaScript
            Secure = true,               // Use true in production with HTTPS
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddHours(2)
        });

        return Ok(new { Message = "Login successful!" });

        // // 5. Return token
        // return Ok(new
        // {
        //     Message = "Login successful!",
        //     Token = new JwtSecurityTokenHandler().WriteToken(token)
        // });
    }

    [HttpGet("users")]
    public IActionResult GetUsers()
    {
        var users = _serviceManager.GetUsers();
        return Ok(users);
    }

    [HttpGet("user")]
    public IActionResult GetUser([FromQuery] string user_id)
    {
        var userid = long.Parse(user_id);
        var user = _serviceManager.GetUserById(userid);
        return Ok(user);
    }

    [HttpGet("stations")]
    public IActionResult GetStations()
    {
        var stations = _serviceManager.GetStations();
        return Ok(stations);
    }

    [HttpGet("station")]
    public IActionResult GetStation([FromQuery] string station_id)
    {
        var stationId = long.Parse(station_id);
        var station = _serviceManager.GetStation(stationId);
        return Ok(station);
    }

    [HttpGet("services")]
    public IActionResult ListServiceInfpp()
    {
        ServiceInfo[] infos = _serviceManager.ListServiceInfo();
        return Ok(infos);
    }
    
    public IActionResult GetSurveyJsConfigForItem (string itemtype)
    {
        throw new NotImplementedException("This method is not implemented yet.");
        //return Ok(Utils.GenerateSurveyJsConfigForItem(itemtype));
    }
}