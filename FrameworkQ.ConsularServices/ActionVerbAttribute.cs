using System;

namespace FrameworkQ.ConsularServices
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ActionVerbAttribute : Attribute
    {
        public string Verb { get; init;}
        public string[] PKs { get; init;}

       
    }
}