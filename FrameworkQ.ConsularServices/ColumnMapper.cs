using System;
using System.Linq.Expressions;
using System.Reflection;
using Dapper.FluentMap;
using Dapper.FluentMap.Mapping;
using FrameworkQ.ConsularServices.Services;
using FrameworkQ.ConsularServices.Users;

namespace FrameworkQ.ConsularServices;

/// <summary>
/// A generic entity map that automatically maps properties to column names
/// specified by the [Column] attribute. This uses reflection to avoid
/// manually mapping each property.
/// </summary>
/// <typeparam name="T">The entity type to map.</typeparam>
public class ColumnMapper<T> : EntityMap<T> where T : class
{
    public ColumnMapper()
    {
        // Get all public instance properties of the entity type T.
        PropertyInfo[] infos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        
        foreach (var propertyInfo in infos)
        {
            // Check if the property has a [Column] attribute.
            var columnAttr = propertyInfo.GetCustomAttribute<System.ComponentModel.DataAnnotations.Schema.ColumnAttribute>();

            if (columnAttr != null)
            {
                // --- Reflection to call Map(lambda).ToColumn(columnName) ---

                // 1. Build the lambda expression for the property access.
                //    The base Map method expects an Expression<Func<T, object>>.
                var parameter = Expression.Parameter(typeof(T), "x");
                var property = Expression.Property(parameter, propertyInfo);
                
                // 2. If the property is a value type (e.g., int, DateTime), it must be
                //    boxed (converted) to an object for the expression to match the required signature.
                var convert = Expression.Convert(property, typeof(object));
                var lambda = Expression.Lambda<Func<T, object>>(convert, parameter);

                // 3. Get the 'Map' method from the base EntityMap<T> class.
                //    It is a protected, non-public instance method.
                var mapMethod = typeof(EntityMap<T>)
                    .GetMethod("Map", BindingFlags.Instance | BindingFlags.NonPublic);

                if (mapMethod == null)
                {
                    // This would only happen if Dapper.FluentMap changes its API, but it's good practice to check.
                    throw new InvalidOperationException("Could not find the 'Map' method via reflection. The Dapper.FluentMap API may have changed.");
                }

                // 4. Invoke the 'Map' method on the current instance ('this') with the created lambda expression.
                //    This returns an IPropertyMap object.
                var propertyMap = mapMethod.Invoke(this, new object[] { lambda });

                // 5. Get the 'ToColumn' method from the returned PropertyMap object.
                var toColumnMethod = propertyMap.GetType()
                    .GetMethod("ToColumn", new[] { typeof(string), typeof(bool) });

                // 6. Invoke the 'ToColumn' method on the PropertyMap object, passing in the column name from the attribute.
                if (toColumnMethod != null)
                {
                    toColumnMethod.Invoke(propertyMap, new object[] { columnAttr.Name, false });
                }
            }
        }
    }

    
    private static bool _isInitialized = false;
    /// <summary>
    /// Initializes Dapper.FluentMap with all the entity types that should be mapped.
    /// This method should be called once at application startup.
    /// </summary>
    public static void MapTypes()
    {
        if (_isInitialized == false)
        {
            FluentMapper.Initialize(cfg =>
            {
                cfg.AddMap(new ColumnMapper<User>());
                cfg.AddMap(new ColumnMapper<Role>());
                cfg.AddMap(new ColumnMapper<Permission>());
                cfg.AddMap(new ColumnMapper<RolePermissionMap>());
                cfg.AddMap(new ColumnMapper<RoleUserMap>());
                cfg.AddMap(new ColumnMapper<Service>());
                cfg.AddMap(new ColumnMapper<ServiceInstance>());
                cfg.AddMap(new ColumnMapper<Station>());
                cfg.AddMap(new ColumnMapper<Token>());
                _isInitialized = true;
            });
        }
    }
}
