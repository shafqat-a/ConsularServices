using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using FrameworkQ.ConsularServices.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations.Schema;
using FrameworkQ.ConsularServices.Services;

namespace FrameworkQ.ConsularServices;

public sealed class EntityRouteRegistry
{
    public IDictionary<string, EntityApiMap> Routes { get; } = new Dictionary<string, EntityApiMap>();
}
public class ControllerHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ControllerHandlerMiddleware> _logger;
    private readonly EntityRouteRegistry _registry;


    public ControllerHandlerMiddleware(
        RequestDelegate next,
        ILogger<ControllerHandlerMiddleware> logger,
        EntityRouteRegistry registry)
    {
        _next = next;
        _logger = logger;
        _registry = registry;
    }

     private static async Task<object?> CallManagerMethodAsync(object manager, EntityApiMap map, HttpRequest rq, CancellationToken ct = default)
    {
        var managerType = manager.GetType();
        
        switch (map.ActionType)
        {
            case ActionTypes.List:
                // Find the ListAsync method with generic parameter
                var listMethod = managerType.GetMethod("ListAsync");
                if (listMethod != null)
                {
                    // Make the generic method with the entity type
                    var genericListMethod = listMethod.MakeGenericMethod(map.EntityType);
                    var listTask = (Task)genericListMethod.Invoke(manager, new object[] { ct })!;
                    await listTask;
                    return listTask.GetType().GetProperty("Result")?.GetValue(listTask);
                }
                throw new InvalidOperationException($"ListAsync method not found on {managerType.Name}");

            case ActionTypes.Get:
                // find the id from the context or route values
                EntityMetaAttribute? entityMeta = Utils.GetAttribute<EntityMetaAttribute>(map.EntityType);
                if (entityMeta == null)
                    throw new InvalidOperationException($"EntityMetaAttribute not found for type {map.EntityType.Name}");

                var idProperty = map.EntityType.GetProperty(entityMeta.PKs[0]);
                if (idProperty == null)
                    throw new InvalidOperationException($"Primary key property '{entityMeta.PKs[0]}' not found on entity {map.EntityType.Name}");
                
                var attrsCol = idProperty.GetCustomAttribute<ColumnAttribute>();
                // Get the ID from the request context
                string idValue = rq.Query[attrsCol?.Name ?? idProperty.Name].ToString() ?? string.Empty;

                if (string.IsNullOrEmpty(idValue))
                    throw new InvalidOperationException($"ID value for '{entityMeta.PKs[0]}' not provided in request");

                // Convert to the appropriate type
                var id = Convert.ChangeType(idValue, idProperty.PropertyType);
                
                // Find the GetAsync method with generic parameter
                var getMethod = managerType.GetMethod("GetAsync");
                if (getMethod != null)
                {
                    // Make the generic method with the entity type
                    var genericGetMethod = getMethod.MakeGenericMethod(map.EntityType);
                    var getTask = (Task)genericGetMethod.Invoke(manager, new object[] { id, ct })!;
                    await getTask;
                    return getTask.GetType().GetProperty("Result")?.GetValue(getTask);
                }
                throw new InvalidOperationException($"GetAsync method not found on {managerType.Name}");

            case ActionTypes.Create:
                var createMethod = managerType.GetMethod("CreateAsync");
                if (createMethod != null)
                {
                    // For create, you'd need to deserialize the request body to the entity
                    throw new NotImplementedException("Create requires request body deserialization");
                }
                throw new InvalidOperationException($"CreateAsync method not found on {managerType.Name}");

            case ActionTypes.Update:
                var updateMethod = managerType.GetMethod("UpdateAsync");
                if (updateMethod != null)
                {
                    // For update, you'd need to deserialize the request body to the entity
                    throw new NotImplementedException("Update requires request body deserialization");
                }
                throw new InvalidOperationException($"UpdateAsync method not found on {managerType.Name}");

            case ActionTypes.Delete:
                var deleteMethod = managerType.GetMethod("DeleteAsync");
                if (deleteMethod != null)
                {
                    // You'd need to extract the ID similar to the Get case
                    throw new NotImplementedException("Delete requires ID extraction");
                }
                throw new InvalidOperationException($"DeleteAsync method not found on {managerType.Name}");

            default:
                throw new NotSupportedException($"Action '{map.ActionType}' is not supported.");
        }
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        string? controller = context.Request.RouteValues["controller"]?.ToString();
        string? action = context.Request.RouteValues["action"]?.ToString();
        var path = context.Request.Path.ToString();
        var method = context.Request.Method;

        var key = $"{method} {path}";
        if (_registry.Routes.TryGetValue(key, out var map))
        {
            _logger.LogInformation("Entity API Hit: {Method} {Path} => {EntityType} ({ActionType})",
                method, path, map.EntityType.Name, map.ActionType);
            var serviceType = typeof(IManager<>).MakeGenericType(map.EntityType);
            var manager = context.RequestServices.GetRequiredService(serviceType);
            if (map.ActionType == ActionTypes.List || map.ActionType == ActionTypes.Get)
            {
                // For list endpoints, action is determined by mapping, not route values
                var result = await CallManagerMethodAsync(manager, map, context.Request, context.RequestAborted);
                _logger.LogInformation("Entity API Handled: {Method} {Path} => {EntityType} ({ActionType})",
                    method, path, map.EntityType.Name, map.ActionType);
                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    context.Response.ContentType = "application/json";
                    if (result != null)
                    {
                        var jsonOptions = context.RequestServices
                            .GetRequiredService<Microsoft.Extensions.Options.IOptions<Microsoft.AspNetCore.Mvc.JsonOptions>>()
                            .Value.JsonSerializerOptions;

                        await JsonSerializer.SerializeAsync(
                            context.Response.Body,
                            result,
                            result.GetType(),
                            jsonOptions,
                            context.RequestAborted);
                    }
                }
                return; // short-circuit pipeline for handled entity list
            }
        }

        var endpoint = context.GetEndpoint();
        bool matched = endpoint != null && !string.IsNullOrEmpty(controller) && !string.IsNullOrEmpty(action);

        if (matched)
            _logger.LogInformation("ControllerAction Start: {Method} {Path} => {Controller}/{Action}", method, path, controller, action);
        else
            _logger.LogInformation("MissedAction Start: {Method} {Path}", method, path);

        try { await _next(context); }
        finally
        {
            sw.Stop();
            if (matched)
                _logger.LogInformation("ControllerAction End: {Method} {Path} => {Controller}/{Action} ({StatusCode}) in {Elapsed}ms",
                    method, path, controller, action, context.Response.StatusCode, sw.ElapsedMilliseconds);
            else
                _logger.LogInformation("MissedAction End: {Method} {Path} ({StatusCode}) in {Elapsed}ms",
                    method, path, context.Response.StatusCode, sw.ElapsedMilliseconds);
        }
    }
}

public static class ControllerHandlerServiceCollectionExtensions
{

    public static IApplicationBuilder UseControllerHandlerLogging(this IApplicationBuilder app)
    {
        // Nothing to register here—container is frozen. Just add middleware.
        return app.UseMiddleware<ControllerHandlerMiddleware>();
    }
    public static IServiceCollection AddControllerHandler(this IServiceCollection services, string prefix = "/api")
    {
        // Register your managers here (pre-build)
        // If ServiceManager implements IManager<User>:
        
        //services.AddScoped<IManager<Service>, ServiceManager>();


        // If you refactor to a generic manager: services.AddScoped(typeof(IManager<>), typeof(ServiceManager<>));

        // Build and register the routes
        var registry = new EntityRouteRegistry();
        MapEntities(prefix, registry.Routes);

        services.AddSingleton(registry);
        return services;
    }

    private static void MapEntities(string prefix, IDictionary<string, EntityApiMap> entityRoutes)
    {
        MapEntity<User>(prefix, entityRoutes);
        MapEntity<Service>(prefix, entityRoutes);
        // MapEntity<OtherEntity>(prefix, entityRoutes); // add more here
    }

    private static void MapEntity<T>(string prefix, IDictionary<string, EntityApiMap> entityRoutes)
    {
        var entityType = typeof(T);
        var meta = entityType.GetCustomAttribute<EntityMetaAttribute>()
                  ?? throw new InvalidOperationException($"EntityMetaAttribute not found for type {entityType.Name}");

        entityRoutes[$"GET {prefix}/{meta.UrlStemList}"] = new EntityApiMap
        {
            ApiPath = $"{prefix}/{meta.UrlStemList}",
            HttpMethod = "GET",
            ActionType = ActionTypes.List,
            EntityType = entityType,
            ManagerType = typeof(IManager<T>)
        };

        entityRoutes[$"GET {prefix}/{meta.UrlStem}"] = new EntityApiMap
        {
            ApiPath = $"{prefix}/{meta.UrlStem}",
            HttpMethod = "GET",
            ActionType = ActionTypes.Get,
            EntityType = entityType,
            ManagerType = typeof(IManager<T>)
        };

        entityRoutes[$"POST {prefix}/{meta.UrlStem}"] = new EntityApiMap
        {
            ApiPath = $"{prefix}/{meta.UrlStem}",
            HttpMethod = "POST",
            ActionType = ActionTypes.Create,
            EntityType = entityType,
            ManagerType = typeof(IManager<T>)
        };

        entityRoutes[$"PUT {prefix}/{meta.UrlStem}"] = new EntityApiMap
        {
            ApiPath = $"{prefix}/{meta.UrlStem}",
            HttpMethod = "PUT",
            ActionType = ActionTypes.Update,
            EntityType = entityType,
            ManagerType = typeof(IManager<T>)
        };

        entityRoutes[$"DELETE {prefix}/{meta.UrlStem}"] = new EntityApiMap
        {
            ApiPath = $"{prefix}/{meta.UrlStem}",
            HttpMethod = "DELETE",
            ActionType = ActionTypes.Delete,
            EntityType = entityType,
            ManagerType = typeof(IManager<T>)
        };
    }
}

