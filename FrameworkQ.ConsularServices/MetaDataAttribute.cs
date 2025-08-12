using System;

namespace FrameworkQ.ConsularServices
{

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MetaDataAttribute : Attribute
    {
        

        public string? Title { get; init; }
        
        public string? Description { get; init; }
        public string? InputType { get; init; }
        public bool IsRequired { get; init; }
        public bool IsVisible { get; init; } = true;
    }
}