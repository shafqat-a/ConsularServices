using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text;
using System.Dynamic;

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

    public static object BuildSurveyJsConfig(string typeName)
    {
        var t = Type.GetType(typeName) 
                ?? throw new ArgumentException($"Type '{typeName}' not found.");
        PropertyInfo[] props = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        object[] elements = new object[props.Length];

        for (int i = 0; i < props.Length; i++)
        {
            PropertyInfo p = props[i];

            // Attributes (as in your original)
            ColumnAttribute? col = GetAttribute<ColumnAttribute>(p);
            PropertyMetaAttribute? meta = GetAttribute<PropertyMetaAttribute>(p);

            // Figure out the *actual* type (unwrap Nullable<T>)
            Type actualType = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;

            // Start with a flexible object so we can add extra fields when it's an enum
            dynamic el = new ExpandoObject();
            var dict = (IDictionary<string, object?>)el;

            dict["type"] = "text";
            dict["name"] = Camelize(p.Name);
            dict["title"] = meta?.Title ?? Humanize(p.Name);
            dict["isRequired"] = meta?.IsRequired ?? false;
            dict["visible"] = meta?.IsVisible ?? true;
            dict["inputType"] = meta?.InputType; // you may keep or drop this for enums

            // If enum (supports nullable enums too)
            if (actualType.IsEnum)
            {
                dict["type"] = "dropdown"; // SurveyJS dropdown
                // Build choices: [{ value, text }]
                dict["choices"] = BuildChoicesFromEnum(actualType);

                // Optional: if the property itself is nullable, you can show a 'None' item
                // dict["showNoneItem"] = true;
                // dict["noneText"] = "(None)";
            }

            elements[i] = el;
        }

        return new { elements };
    }

    private static List<object> BuildChoicesFromEnum(Type enumType)
    {
        // values = underlying numeric array; iterate as enum to get names safely
        var values = Enum.GetValues(enumType); // docs: GetValues returns all members’ values
        var result = new List<object>(values.Length);

        foreach (var v in values) // v is boxed enum value
        {
            string name = Enum.GetName(enumType, v)!; // display text (you can Humanize if desired)

            // If you want numeric values in the JSON, convert to the enum’s underlying type:
            // var numeric = Convert.ChangeType(v, Enum.GetUnderlyingType(enumType));
            // If you prefer string values (names), set `value = name`.
            var numeric = Convert.ChangeType(v, Enum.GetUnderlyingType(enumType)); // value as number

            result.Add(new { value = numeric, text = name });
        }

        return result;
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