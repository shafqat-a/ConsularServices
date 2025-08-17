using System.Security.Cryptography.X509Certificates;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FrameworkQ.ConsularServices
{
    public enum ActionTypes
    {
        Create,
        Get,
        Update,
        Delete,
        List,
        Custom
    }

    public class EntityApiMap
    {
        public string ApiPath { get; set; }
        public string HttpMethod { get; set; }
        public ActionTypes ActionType { get; set; }

        public string? CustomAction { get; set; }
        public Type EntityType { get; set; }    
        public Type ManagerType { get; set; }
        
        

    }
}