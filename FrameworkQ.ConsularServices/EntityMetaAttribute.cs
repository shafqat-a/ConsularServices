using System;

namespace FrameworkQ.ConsularServices
{

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class EntityMetaAttribute : Attribute
    {
        public string? UrlStem { get; set; }
        public string Verb { get; init;}
        public string[] PKs { get; init;}

     
    }
}