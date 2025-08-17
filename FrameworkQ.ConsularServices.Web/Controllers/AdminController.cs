using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FrameworkQ.ConsularServices.Web.Controllers;

//[WebControllerActionInterruptFilter]
//[ServiceFilter(typeof(WebControllerActionInterruptFilter))]
public class AdminController : Controller
{

    [HttpGet("/users")]
    public IActionResult Users()
    {
        return View("ListView");
    }

    [HttpGet("/stations")]
    public IActionResult Stations()
    {
        return View("ListView");
    }

    [HttpPost("/station")]
    public IActionResult Station()
    {
        return View();
    }

    [HttpPost("/user")]
    public IActionResult User([FromForm(Name = "user_id")] string userId)
    {
        return View("ItemView");
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