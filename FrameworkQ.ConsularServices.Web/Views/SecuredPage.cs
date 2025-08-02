using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FrameworkQ.ConsularServices.Web.Pages;

[Authorize]
public class SecuredPage: PageModel
{
    
}