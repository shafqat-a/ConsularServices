using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FrameworkQ.ConsularServices.Web.Controllers;

public class AdminController : Controller
{

    [HttpGet("/users")]
    public IActionResult Users()
    {
        return View();
    }

    [HttpGet("/stations")]
    public IActionResult Stations()
    {
        return View();
    }

    [HttpPost("/station")]
    public IActionResult Station()
    {
        return View();
    }

    [HttpPost("/user")]
    public IActionResult EditUser([FromForm(Name = "user_id")] string userId)
    {
        ViewBag.UserId = userId.ToString();
        return View();
    }
    
    [HttpGet("/services")]
    public IActionResult Services()
    {
        return View("ListView");
    }
    
    [HttpPost("/service")]
    public IActionResult Service()
    {
        return View("ItemView");
    }

    [HttpGet("/form-designer")]
    public IActionResult FormDesigner()
    {
        return View("FormDesigner");
    }
}