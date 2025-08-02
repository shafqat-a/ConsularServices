using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FrameworkQ.ConsularServices.Web.Controllers;

public class AdminController: Controller
{
    
    [HttpGet("/users")]
    public IActionResult Users()
    {
        return View();
    }
}