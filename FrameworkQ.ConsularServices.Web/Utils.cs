using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text;

namespace FrameworkQ.ConsularServices.Web;

public static class Utils
{

    private static readonly string _Version = "1.0.2"; // Example version, replace with actual version logic
    public static string MakeVersionable(string path)
    {

        if (string.IsNullOrEmpty(_Version))
        {
            return path;
        }
        return $"{path}?v={_Version}";

    }

    public static string GenerateItemPageHiddenIdString(string typeName, Microsoft.AspNetCore.Http.IQueryCollection query)
    {

        // <input type="hidden" name="user_id" id="user_id" value="@ViewBag.UserId" />

        Type itemType = GetTypeFromName(typeName);
        ActionVerbAttribute? actionVerb =null;
        try{
            actionVerb = itemType.GetCustomAttribute<ActionVerbAttribute>();
        } catch (Exception ex){
            // Handle the exception (e.g., log it)
        }

        StringBuilder sb = new StringBuilder();
        foreach (string pk in actionVerb.PKs)
        {
            PropertyInfo? prop = itemType.GetProperty(pk);
            ColumnAttribute? col = prop?.GetCustomAttribute<ColumnAttribute>();
            var value =  query[col.Name].ToString() ?? string.Empty;
            if (col != null)
            {
                sb.Append($"<input type='hidden' name='{col.Name}' id='{col.Name}' value='{value}' />");
            }
        }

        // Return the HTML string for the hidden input
        return sb.ToString();
    }

    public static Type? GetTypeFromName(string typeName)
    {
        Type? typInput = null;
        string typenameToTry = "FrameworkQ.ConsularServices.Users." + UppercaseFirstLetterWord(typeName);
        typenameToTry += ",FrameworkQ.ConsularServices";
        try {
            typInput = Type.GetType(typenameToTry);
        } catch  (Exception ex){
        }

        try{
            if (typInput ==null){
                typenameToTry = "FrameworkQ.ConsularServices.Services." + UppercaseFirstLetterWord(typeName);
                typenameToTry += ",FrameworkQ.ConsularServices";
                typInput = Type.GetType(typenameToTry);

            }
        } catch (Exception ex) {
            // Handle the exception (e.g., log it)
        }

        return typInput;
    }

    public static object BuildSurveyJsConfig(string typename)
    {

        // 1. Gather the properties once
        PropertyInfo[] props = Type.GetType(typename).GetProperties(
                                   BindingFlags.Public | BindingFlags.Instance);

        // 2. Pre-allocate the array
        object[] elements = new object[props.Length];

        // 3. Fill the array with plain loops
        for (int i = 0; i < props.Length; i++)
        {
            PropertyInfo p = props[i];

            // Attributes
            ColumnAttribute? col = GetAttribute<ColumnAttribute>(p);
            MetaDataAttribute? meta = GetAttribute<MetaDataAttribute>(p);

            // Build the element object
            elements[i] = new
            {
                type = "text",
                name = (p.Name),
                title = meta?.Title ?? Humanize(p.Name),
                isRequired = meta?.IsRequired ?? false,
                visible = meta?.IsVisible ?? true,
                inputType = meta?.InputType
            };
        }

        // 4. Return the final shape
        return new
        {
            elements
        };
    }

     private static TAttr? GetAttribute<TAttr>(PropertyInfo prop) where TAttr : Attribute
    {
        object[] attrs = prop.GetCustomAttributes(typeof(TAttr), inherit: false);
        return attrs.Length > 0 ? (TAttr)attrs[0] : null;
    }

    
    // Simple camelCase conversion (lowercase first char)
    public static string Camelize(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;
        return char.ToLowerInvariant(name[0]) + name.Substring(1);
    }

    public static string UppercaseFirstLetterWord ( string name ){
       if (string.IsNullOrEmpty(name)) return name;
       return char.ToUpperInvariant(name[0]) + name.Substring(1);
    }

    // Very small humanizer: split on uppercase letters
    public static string Humanize(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;

        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < name.Length; i++)
        {
            if (i > 0 && char.IsUpper(name[i]))
                sb.Append(' ');
            sb.Append(name[i]);
        }
        return sb.ToString();
    }
}