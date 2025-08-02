using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using FrameworkQ.ConsularServices.Web.Api.Contracts;
using System.Text;


namespace FrameworkQ.ConsularServices.Web.Controllers
{
    public class HomeController : Controller
    {
        private IServiceManager _serviceManager;
        public HomeController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }
        
        [HttpGet("/")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("/dashboard")]
        public IActionResult Dashboard()
        {
            return View("Index");
        }
        
        [HttpGet("/login")]
        public IActionResult Login()
        {
            return View();
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

        [HttpPost("/login")]
        public async Task<IActionResult> Login([FromForm] string email, [FromForm] string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View();
            }

            var user = _serviceManager.GetUserByEmail(email);
            var hashedPassword = Hash256SHA(password);
            if (user == null || user.PasswordHash.ToUpper() != hashedPassword.ToUpper())
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View();
            }

            var claims = new[]
            {
                new Claim(System.Security.Claims.ClaimTypes.Name, user.Email),
                // Add other claims as needed
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return LocalRedirect("/");
        }

        [HttpGet("/logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return View();
        }


    }
}
