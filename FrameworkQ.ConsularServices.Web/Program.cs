using System.Text;
using FrameworkQ.ConsularServices.Web;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// 1. Add services for Razor Pages
builder.Services.AddRazorPages();
// Program.cs
builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection("Jwt"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();

        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwt.Key)),
            ValidateIssuer   = true,
            ValidIssuer      = jwt.Issuer,
            ValidateAudience = true,
            ValidAudience    = jwt.Audience,
            ClockSkew        = TimeSpan.Zero
        };
    });


var app = builder.Build();

// 2. Configure the app to use static files and routing
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "www")),
    RequestPath = ""
});
app.UseRouting();

// 3. Map the Razor Pages. This will look for an Index.cshtml file.
//    We can remove the old "Hello World" endpoint.
app.MapRazorPages();
app.MapControllers();

app.Run();