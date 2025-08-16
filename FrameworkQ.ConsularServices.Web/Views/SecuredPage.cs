using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FrameworkQ.ConsularServices.Web.Pages;


public class SecuredPage: PageModel
{
    public IHttpContextAccessor _httpContextAccessor;
    public SecuredPage(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public HttpContext HttpContext => _httpContextAccessor.HttpContext;
    
    public string GenerateItemPageHiddenIdString(string typeName = "")
    {
        if (string.IsNullOrEmpty(typeName))
        {
            // Parameter not passed. Infer from action verb
            typeName = GetActionWordFromRoute();
        }

        // <input type="hidden" name="user_id" id="user_id" value="@ViewBag.UserId" />

        Type itemType = GetTypeFromName(typeName);
        EntityMetaAttribute? actionVerb =null;
        try{
            actionVerb = itemType.GetCustomAttribute<EntityMetaAttribute>();
        } catch (Exception ex){
            // Handle the exception (e.g., log it)
        }

        
        
        StringBuilder sb = new StringBuilder();
        foreach (string pk in actionVerb.PKs)
        {
            PropertyInfo? prop = itemType.GetProperty(pk);
            ColumnAttribute? col = prop?.GetCustomAttribute<ColumnAttribute>();
            var value =  HttpContext.Request.Form[col.Name].ToString() ?? string.Empty;
            
            if (col != null)
            {
                sb.Append($"<input type='hidden' name='{col.Name}' id='{col.Name}' value='{value}' />");
            }
        }

        // Return the HTML string for the hidden input
        return sb.ToString();
    }

    private Type? GetTypeFromName(string typeName)
    {
        Type? typInput = null;
        string typenameToTry = "FrameworkQ.ConsularServices.Users." + Utils.UppercaseFirstLetterWord(typeName);
        typenameToTry += ",FrameworkQ.ConsularServices";
        try {
            typInput = Type.GetType(typenameToTry);
        } catch  (Exception ex){
        }

        try{
            if (typInput ==null){
                typenameToTry = "FrameworkQ.ConsularServices.Services." + Utils.UppercaseFirstLetterWord(typeName);
                typenameToTry += ",FrameworkQ.ConsularServices";
                typInput = Type.GetType(typenameToTry);

            }
        } catch (Exception ex) {
            // Handle the exception (e.g., log it)
        }

        return typInput;
    }
    
    public string GetActionWordFromRoute()
    {
        var routeData = HttpContext.GetRouteData();
        if (routeData.Values.TryGetValue("action", out var action))
        {
            return action?.ToString() ?? string.Empty;
        }
        return string.Empty;
    }
    
}