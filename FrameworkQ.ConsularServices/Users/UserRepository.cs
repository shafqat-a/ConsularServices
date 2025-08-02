using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using Microsoft.Extensions.Configuration;
using Dapper;
using FrameworkQ.ConsularServices;
using FrameworkQ.ConsularServices.Users;


public interface IUserRepository
{
    Role CreateRole(Role role);
    Role GetRole(long id);
    Role GetRoleByName(string roleName);
    void UpdateRole(Role role);
    void DeleteRole(long id);
    Permission CreatePermission(Permission permission);
    Permission GetPermission(long id);
    List<Permission> GetAllPermissions();
    void UpdatePermission(Permission permission);
    void DeletePermission(long id);
    User CreateUser(User user);
    User GetUser(long id);
    User[] GetUsers();
    User GetUserByEmail(string email);
    void UpdateUser(User user);
    void DeleteUser(long id);
    void AssignPermissionsToRole(long roleId, IEnumerable<long> permissionIds);
    void AssignUsersToRole(long roleId, IEnumerable<long> userIds);
    void AssignRolesToUser(long userId, IEnumerable<long> roleIds);
    void AddPermissionsToRole(long roleId, IEnumerable<long> permissionIds);
    void RemovePermissionsFromRole(long roleId, IEnumerable<long> permissionIds);
    void AddUsersToRole(long roleId, IEnumerable<long> userIds);
    void RemoveUsersFromRole(long roleId, IEnumerable<long> userIds);
   
    void AddRolesToUser(long userId, IEnumerable<long> roleIds);
    void RemoveRolesFromUser(long userId, IEnumerable<long> roleIds);
    bool HasPermission(long userId, long permissionId);
    bool HasPermission(long userId, string permissionName);
}

public class UserRepository : IUserRepository
{
    private readonly string _connectionString;

    public UserRepository(ConnectionProvider connectionProvider)
    {
        _connectionString = connectionProvider.GetConnectionString();
    }

   // ---------- Role CRUD ----------
   public Role CreateRole(Role role)
   {
       const string sql = @"
        INSERT INTO public.role (role_name) 
        VALUES (@RoleName) 
        RETURNING role_id;";

       using var conn = new NpgsqlConnection(_connectionString);

       // Dapper executes the insert and returns the new role_id
       var newId = conn.QuerySingle<long>(sql, role);

       // Assign the newly generated ID to the input object
       role.RoleID = newId;

       return role;
   }

   public Role GetRole(long id)
   {
       const string sql = "SELECT * FROM public.role WHERE role_id = @id;";
       using var conn = new NpgsqlConnection(_connectionString);
       Role role = conn.QueryFirstOrDefault<Role>(sql, new { id });
       return role;
   }

   public Role GetRoleByName(string roleName)
   {
       const string sql = "SELECT * FROM public.role WHERE role_name = @roleName;";
       using var conn = new NpgsqlConnection(_connectionString);
       return conn.QueryFirstOrDefault<Role>(sql, new { roleName });
   }

   public void UpdateRole(Role role)
   {
       const string sql = @"
        UPDATE public.role 
        SET role_name = @RoleName 
        WHERE role_id = @RoleID;";

       using var conn = new NpgsqlConnection(_connectionString);
       conn.Execute(sql, role);
   }

   public void DeleteRole(long id)
   {
       const string sql = "DELETE FROM public.role WHERE role_id = @id;";
       using var conn = new NpgsqlConnection(_connectionString);
       conn.Execute(sql, new { id });
   }

    // ---------- Permission CRUD ----------
    /// <summary>
    /// Creates a new permission in the database.
    /// </summary>
    /// <param name="permission">The permission object containing the ID and name to create.</param>
    /// <returns>The permission object that was successfully saved.</returns>
    public Permission CreatePermission(Permission permission)
    {
        const string sql = @"
        INSERT INTO public.permission (permission_id, permission_name) 
        VALUES (@PermissionID, @PermissionName);";

        using var conn = new NpgsqlConnection(_connectionString);
        conn.Execute(sql, permission);
    
        return permission;
    }

    public Permission GetPermission(long id)
    {
        const string sql = "SELECT * FROM public.permission WHERE permission_id = @id;";
        using var conn = new NpgsqlConnection(_connectionString);
        return conn.QueryFirstOrDefault<Permission>(sql, new { id });
    }

    public List<Permission> GetAllPermissions()
    {
        const string sql = "SELECT * FROM public.permission ORDER BY permission_id;";
        using var conn = new NpgsqlConnection(_connectionString);
        return conn.Query<Permission>(sql).ToList();
    }

    public void UpdatePermission(Permission permission)
    {
        const string sql = @"
        UPDATE public.permission 
        SET permission_name = @PermissionName 
        WHERE permission_id = @PermissionID;";

        using var conn = new NpgsqlConnection(_connectionString);
        conn.Execute(sql, permission);
    }

    public void DeletePermission(long id)
    {
        const string sql = "DELETE FROM public.permission WHERE permission_id = @id;";
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Execute(sql, new { id });
    }

    // ---------- User CRUD ----------
    
    
    public User CreateUser(User user)
    {
        using var conn = new NpgsqlConnection(_connectionString);

        const string sql = @"
        INSERT INTO public.user (name, email, password_hash)
        VALUES (@Name, @Email, @PasswordHash)
        RETURNING user_id;";

        
        user.UserId =  conn.ExecuteScalar<long>(sql, new
        {
            user.Name,
            user.Email,
            user.PasswordHash
        });
        return user;
    }

    public User? GetUser(long id)
    {
        const string sql = "SELECT * FROM public.user WHERE user_id = @Id";

        using var conn = new NpgsqlConnection(_connectionString);
        return conn.QueryFirstOrDefault<User>(sql, new { Id = id });
    }

    public User? GetUserByEmail(string email)
    {
        const string sql = @"
        SELECT user_id,
               name,
               email,
               password_hash
        FROM   public.user
        WHERE  email = @Email";

        using var conn = new NpgsqlConnection(_connectionString);
        return conn.QueryFirstOrDefault<User>(sql, new { Email = email });
    }

    public void UpdateUser(User user)
    {
        const string sql = @"
        UPDATE public.user
        SET    name         = @Name,
               email        = @Email,
               password_hash = @PasswordHash
        WHERE  user_id      = @UserId";

        using var conn = new NpgsqlConnection(_connectionString);
        conn.Execute(sql, user);   // Dapper uses the [Column] attributes to map the object's properties
    }

    public void DeleteUser(long id)
    {
        const string sql = "DELETE FROM public.user WHERE user_id = @UserId";

        using var conn = new NpgsqlConnection(_connectionString);
        conn.Execute(sql, new { UserId = id });
    }

    public User[] GetUsers()
    {
        const string sql = "SELECT * FROM public.user";

        using var conn = new NpgsqlConnection(_connectionString);
        return conn.Query<User>(sql).ToArray();
    }
    
    
    // ---------- Role-Permission / User mapping methods ----------
    public void AssignPermissionsToRole(long roleId, IEnumerable<long> permissionIds)
    {
        const string deleteSql = "DELETE FROM public.role_permission_map WHERE role_id = @roleId;";
        const string insertSql = @"
        INSERT INTO public.role_permission_map (role_id, permission_id) 
        VALUES (@roleId, @permissionId);";

        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        using var tran = conn.BeginTransaction();

        try
        {
            // Step 1: Delete existing permissions within the transaction.
            conn.Execute(deleteSql, new { roleId }, transaction: tran);

            // Step 2: Perform a batch insert for all new permissions.
            // This is much faster than inserting in a loop.
            if (permissionIds?.Any() == true)
            {
                var newPermissions = permissionIds.Select(pid => new { roleId, permissionId = pid });
                conn.Execute(insertSql, newPermissions, transaction: tran);
            }

            // Step 3: If everything succeeded, commit the changes.
            tran.Commit();
        }
        catch
        {
            // Step 4: If any error occurred, roll back all changes.
            tran.Rollback();
            throw; // Re-throw the exception to let the caller know something went wrong.
        }
    }

   
    
    
    public void AssignUsersToRole(long roleId, IEnumerable<long> userIds)
    {
        const string deleteSql = "DELETE FROM public.role_user_map WHERE role_id = @roleId;";
        const string insertSql = @"
        INSERT INTO public.role_user_map (role_id, user_id) 
        VALUES (@roleId, @userId);";

        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        using var tran = conn.BeginTransaction();

        try
        {
            // Step 1: Delete existing users for this role.
            conn.Execute(deleteSql, new { roleId }, transaction: tran);

            // Step 2: Perform an efficient batch insert for all new users.
            if (userIds?.Any() == true)
            {
                var newUsers = userIds.Select(uid => new { roleId, userId = uid });
                conn.Execute(insertSql, newUsers, transaction: tran);
            }

            // Step 3: If all steps succeeded, commit the transaction.
            tran.Commit();
        }
        catch
        {
            // Step 4: If any step failed, roll back all changes.
            tran.Rollback();
            throw;
        }
    }
    
    
    public void AssignRolesToUser(long userId, IEnumerable<long> roleIds)
    {
        const string deleteSql = "DELETE FROM public.role_user_map WHERE user_id = @userId;";
        const string insertSql = @"
        INSERT INTO public.role_user_map (user_id, role_id) 
        VALUES (@userId, @roleId);";

        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        using var tran = conn.BeginTransaction();

        try
        {
            // Step 1: Delete the user's existing roles.
            conn.Execute(deleteSql, new { userId }, transaction: tran);

            // Step 2: Perform an efficient batch insert for all new roles.
            if (roleIds?.Any() == true)
            {
                var newRoles = roleIds.Select(rid => new { userId, roleId = rid });
                conn.Execute(insertSql, newRoles, transaction: tran);
            }

            // Step 3: If successful, commit all changes.
            tran.Commit();
        }
        catch
        {
            // Step 4: If any error occurs, roll back the entire operation.
            tran.Rollback();
            throw;
        }
    }
    public void AddPermissionsToRole(long roleId, IEnumerable<long> permissionIds)
    {
        // Return early if there's nothing to add.
        if (permissionIds?.Any() != true)
        {
            return;
        }

        const string sql = @"
        INSERT INTO public.role_permission_map (role_id, permission_id) 
        VALUES (@roleId, @permissionId) 
        ON CONFLICT (role_id, permission_id) DO NOTHING;";

        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        using var tran = conn.BeginTransaction();
    
        try
        {
            var permissionsToAdd = permissionIds.Select(pid => new { roleId, permissionId = pid });
            conn.Execute(sql, permissionsToAdd, transaction: tran);
            tran.Commit();
        }
        catch
        {
            tran.Rollback();
            throw;
        }
    }

    public void RemovePermissionsFromRole(long roleId, IEnumerable<long> permissionIds)
    {
        // If the list of IDs is null or empty, there's nothing to do.
        if (permissionIds?.Any() != true)
        {
            return;
        }

        const string sql = @"
        DELETE FROM public.role_permission_map 
        WHERE role_id = @roleId AND permission_id = ANY(@permissionIds);";

        using var conn = new NpgsqlConnection(_connectionString);
        conn.Execute(sql, new { roleId, permissionIds });
    }

    public void AddUsersToRole(long roleId, IEnumerable<long> userIds)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        foreach (var uid in userIds)
        {
            conn.Execute("INSERT INTO role_user_map (role_id, user_id) VALUES (@RoleId, @UserId) ON CONFLICT DO NOTHING",
                new { RoleId = roleId, UserId = uid });
        }
    }

    public void RemoveUsersFromRole(long roleId, IEnumerable<long> userIds)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        foreach (var uid in userIds)
        {
            conn.Execute("DELETE FROM role_user_map WHERE role_id = @RoleId AND user_id = @UserId",
                new { RoleId = roleId, UserId = uid });
        }
    }
    /// <summary>
    /// Adds one or more roles to a user, ignoring any duplicates.
    /// </summary>
    /// <param name="userId">The ID of the user to add roles to.</param>
    /// <param name="roleIds">A collection of role IDs to add.</param>
    public void AddRolesToUser(long userId, IEnumerable<long> roleIds)
    {
        // Return early if there's nothing to add.
        if (roleIds?.Any() != true)
        {
            return;
        }

        const string sql = @"
        INSERT INTO public.role_user_map (user_id, role_id)
        VALUES (@userId, @roleId)
        ON CONFLICT (user_id, role_id) DO NOTHING;";

        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        using var tran = conn.BeginTransaction();
    
        try
        {
            var rolesToAdd = roleIds.Select(rid => new { userId, roleId = rid });
            conn.Execute(sql, rolesToAdd, transaction: tran);
            tran.Commit();
        }
        catch
        {
            tran.Rollback();
            throw;
        }
    }

    public void RemoveRolesFromUser(long userId, IEnumerable<long> roleIds)
    {
        // If the list of IDs is null or empty, there's nothing to do.
        if (roleIds?.Any() != true)
        {
            return;
        }

        const string sql = @"
        DELETE FROM public.role_user_map 
        WHERE user_id = @userId AND role_id = ANY(@roleIds);";

        using var conn = new NpgsqlConnection(_connectionString);
        conn.Execute(sql, new { userId, roleIds });
    }
    
    
    /// <summary>
    /// Checks if a user has a specific permission through their assigned roles.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="permissionId">The ID of the permission to check for.</param>
    /// <returns>True if the user has the permission, otherwise false.</returns>
    public bool HasPermission(long userId, long permissionId)
    {
        const string sql = @"
        SELECT EXISTS (
            SELECT 1
            FROM public.role_user_map rum
            INNER JOIN public.role_permission_map rpm ON rum.role_id = rpm.role_id
            WHERE rum.user_id = @userId AND rpm.permission_id = @permissionId
        );";

        using var conn = new NpgsqlConnection(_connectionString);
        return conn.ExecuteScalar<bool>(sql, new { userId, permissionId });
    }
    
    /// <summary>
    /// Checks if a user has a specific permission by the permission's name.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="permissionName">The name of the permission to check for.</param>
    /// <returns>True if the user has the permission, otherwise false.</returns>
    public bool HasPermission(long userId, string permissionName)
    {
        const string sql = @"
        SELECT EXISTS (
            SELECT 1
            FROM public.role_user_map rum
            INNER JOIN public.role_permission_map rpm ON rum.role_id = rpm.role_id
            INNER JOIN public.permission p ON rpm.permission_id = p.permission_id
            WHERE rum.user_id = @userId AND p.permission_name = @permissionName
        );";

        using var conn = new NpgsqlConnection(_connectionString);
        return conn.ExecuteScalar<bool>(sql, new { userId, permissionName });
    }
    
    
    
    
    
}
