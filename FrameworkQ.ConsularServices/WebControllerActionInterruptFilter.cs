using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class WebControllerActionInterruptFilter :System.Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        Console.WriteLine("WebControllerActionInterruptFilter: OnActionExecutionAsync called");
        Console.WriteLine ("\t ---" +context.Controller.GetType().Name + " - " + context.ActionDescriptor.DisplayName);
        Console.WriteLine ("\t ---"+ context.HttpContext.Request.Method + " - " + context.HttpContext.Request.Path);

        // Logic to interrupt the action execution
        await next();
    }
}