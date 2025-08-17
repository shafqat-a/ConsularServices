namespace FrameworkQ.ConsularServices;

using System;
using System.Reflection;

public static class Utils
{
    public static string UppercaseFirstLetterWord(string word)
    {
        if (string.IsNullOrEmpty(word))
            return word;

        return char.ToUpper(word[0]) + word.Substring(1);
    }

    public static TAttr? GetAttribute<TAttr>(PropertyInfo prop) where TAttr : Attribute
    {
        object[] attrs = prop.GetCustomAttributes(typeof(TAttr), inherit: false);
        return attrs.Length > 0 ? (TAttr)attrs[0] : null;
    }

    public static TAttr? GetAttribute<TAttr>(Type type) where TAttr : Attribute
    {
        object[] attrs = type.GetCustomAttributes(typeof(TAttr), inherit: false);
        return attrs.Length > 0 ? (TAttr)attrs[0] : null;
    }
}