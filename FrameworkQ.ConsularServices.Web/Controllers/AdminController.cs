using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FrameworkQ.ConsularServices.Web.Controllers;

public class AdminController: Controller
{
    
    [HttpGet("/user")]
    public IActionResult User()
    {
        return View();
    }
}