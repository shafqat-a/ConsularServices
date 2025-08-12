using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FrameworkQ.ConsularServices.Web.Pages;

public class UnsecuredPage : PageModel
{

    public IHttpContextAccessor _httpContextAccessor;
    UnsecuredPage(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public HttpContext HttpContext => _httpContextAccessor.HttpContext;


    public Dictionary<string, string> RequestParams { get; set; } 
    public void OnGet()
    {
        this.RequestParams = new Dictionary<string, string>();
        // This method can be used to handle GET requests for unsecured pages
        // You can add any initialization logic here if needed
        var q = HttpContext.Request.Query;
        for (int i = 0; i < q.Count; i++)
        {
            var kvp   = q.ElementAt(i);
            string k  = kvp.Key;
            var vals  = kvp.Value;
            if (vals.Count ==1) {
                this.RequestParams.Add(k, vals[0]);
            }// Skip if no values
            
        }
    }

    
}