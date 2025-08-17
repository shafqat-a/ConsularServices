using System.Diagnostics;
using System.Reflection;
using FrameworkQ.ConsularServices.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FrameworkQ.ConsularServices;

public class ControllerHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ControllerHandlerMiddleware> _logger;
    internal static IDictionary<string, EntityApiMap> _EntityRoutes;
    public ControllerHandlerMiddleware(RequestDelegate next, ILogger<ControllerHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        string? controller = context.Request.RouteValues["controller"]?.ToString();
        string? action = context.Request.RouteValues["action"]?.ToString();
        var path = context.Request.Path.ToString();
        var method = context.Request.Method;

        var key = $"{method} {path}";
        if (ControllerHandlerMiddleware._EntityRoutes.ContainsKey(key))
        {
            _logger.LogInformation("Entity API Hit: {Method} {Path} => {EntityType} ({ActionType})", method, path, ControllerHandlerMiddleware._EntityRoutes[key].EntityType.Name, ControllerHandlerMiddleware._EntityRoutes[key].ActionType);  
        }

        var endpoint = context.GetEndpoint();
        bool matched = endpoint != null && !string.IsNullOrEmpty(controller) && !string.IsNullOrEmpty(action);

        if (matched)
        {
            _logger.LogInformation("ControllerAction Start: {Method} {Path} => {Controller}/{Action}", method, path, controller, action);
        }
        else
        {
            _logger.LogInformation("MissedAction Start: {Method} {Path} (No controller/action matched)", method, path);
        }

        try
        {
            await _next(context);
        }
        finally
        {
            sw.Stop();
            if (matched)
            {
                _logger.LogInformation("ControllerAction End: {Method} {Path} => {Controller}/{Action} ({StatusCode}) in {Elapsed}ms", method, path, controller, action, context.Response.StatusCode, sw.ElapsedMilliseconds);
            }
            else
            {
                _logger.LogInformation("MissedAction End: {Method} {Path} ({StatusCode}) in {Elapsed}ms", method, path, context.Response.StatusCode, sw.ElapsedMilliseconds);
            }
        }
    }
}

public static class ControllerHandlerMiddlewareExtensions
{
    
    public static IApplicationBuilder UseControllerHandlerLogging(this IApplicationBuilder app)
    {
        MapEntities(app, "/api");
        return app.UseMiddleware<ControllerHandlerMiddleware>();
    }

    private static void MapEntities(IApplicationBuilder app, string prefix)
    {
        ControllerHandlerMiddleware._EntityRoutes = new Dictionary<string, EntityApiMap>();
    
        MapEntity<User>(app, prefix, ControllerHandlerMiddleware._EntityRoutes);

    }

    private static void MapEntity<T>(IApplicationBuilder app, string prefix,IDictionary<string,  EntityApiMap> entityRoutes)
    {
        // Find the EntityMetaAttribute for the type T
        Type entityType = typeof(T);
        var entityMeta = entityType.GetCustomAttribute<EntityMetaAttribute>();
        if (entityMeta == null)
        {
            throw new InvalidOperationException($"EntityMetaAttribute not found for type {entityType.Name}");
        }
        
       entityRoutes["GET " + $"{prefix}/" + entityMeta.UrlStemList ] = new EntityApiMap
        {
            ApiPath = $"{prefix}/" +  entityMeta.UrlStemList,
            HttpMethod = "GET",
            ActionType = ActionTypes.List,
            EntityType = entityType
        };

        entityRoutes["GET " + $"{prefix}/" + entityMeta.UrlStem ] = new EntityApiMap
        {
            ApiPath = $"{prefix}/" + entityMeta.UrlStem,
            HttpMethod = "GET",
            ActionType = ActionTypes.Read,
            EntityType = entityType
        };

        entityRoutes["POST " +  $"{prefix}/" + entityMeta.UrlStem ] = new EntityApiMap
        {
            ApiPath =$"{prefix}/" + entityMeta,
            HttpMethod = "POST",
            ActionType = ActionTypes.Create,
            EntityType = entityType
        };

        entityRoutes["UPDATE " +  $"{prefix}/" + entityMeta.UrlStem ] = new EntityApiMap
        {
            ApiPath = $"{prefix}/"+ entityMeta,
            HttpMethod = "PUT",
            ActionType = ActionTypes.Update,
            EntityType = entityType
        };

        entityRoutes["DELETE " +  $"{prefix}/" + entityMeta.UrlStem ] = new EntityApiMap
        {
            ApiPath = $"{prefix}/" + entityMeta,
            HttpMethod = "DELETE",
            ActionType = ActionTypes.Update,
            EntityType = entityType
        };

    
    }

    
}
