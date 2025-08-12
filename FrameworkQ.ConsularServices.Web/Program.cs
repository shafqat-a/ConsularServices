using System.Text;
using FrameworkQ.ConsularServices;
using FrameworkQ.ConsularServices.Users;
using FrameworkQ.ConsularServices.Web;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// 1. Add services for Razor Pages
builder.Services.AddControllersWithViews();
// Program.cs
builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection("Jwt"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,
            ValidateAudience = true,
            ValidAudience = jwt.Audience,
            ClockSkew = TimeSpan.Zero
        };

        // ✅ Read token from cookie named "jwt"
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Cookies["jwt"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }
                return Task.CompletedTask;
            }
        };
    });     

// In Program.cs or Startup.cs
builder.Services.AddSingleton(sp => 
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("Connection string 'DefaultConnection' not found in configuration.");
    }
    return connectionString;
});

// If you're using a repository pattern with dependency injection

DependencyInjection.AddConsularServices(builder.Services);
builder.Services.AddHttpContextAccessor();
var app = builder.Build();

// 2. Configure the app to use static files and routing
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "www")),
    RequestPath = "",
    OnPrepareResponse = ctx => {
    ctx.Context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate, max-age=0";
    ctx.Context.Response.Headers["Pragma"]        = "no-cache";
    ctx.Context.Response.Headers["Expires"]       = "-1";
  }
});
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// 3. Map the Razor Pages. This will look for an Index.cshtml file.
//    We can remove the old "Hello World" endpoint.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllers();
ColumnMapper<User>.MapTypes();

app.Run(); 