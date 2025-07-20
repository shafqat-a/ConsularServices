using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using Microsoft.Extensions.Configuration;
using Dapper;
using FrameworkQ.ConsularServices;
using FrameworkQ.ConsularServices.Users;


public class UserRepository : IUserRepository
{
    private readonly string _connectionString;

    public UserRepository()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json");
        var configuration = builder.Build();
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    // Role CRUD
    public void CreateRole(Role role)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        conn.Execute("INSERT INTO Roles (Name) VALUES (@Name)", role);
    }
    
    public User GetUserByEmail(string email)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        // This query assumes the 'Username' column in the database stores the user's email.
        // It also aliases the database columns to match the 'User' class property names for Dapper's mapping.
        const string sql = @"
            SELECT 
                UserId , 
                Name, 
                PasswordHash 
            FROM Users 
            WHERE Email = @Email";
            
        return conn.QueryFirstOrDefault<User>(sql, new { Email = email });
    }

    public Role GetRole(int id)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        return conn.QueryFirstOrDefault<Role>("SELECT Id, Name FROM Roles WHERE Id = @Id", new { Id = id });
    }

    public void UpdateRole(Role role)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        conn.Execute("UPDATE Roles SET Name = @Name WHERE Id = @Id", role);
    }

    public void DeleteRole(int id)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        using var cmd = new NpgsqlCommand("DELETE FROM Roles WHERE Id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.ExecuteNonQuery();
    }

    // Permission CRUD
    public void CreatePermission(Permission permission)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        using var cmd = new NpgsqlCommand("INSERT INTO Permissions (Name) VALUES (@Name)", conn);
        cmd.Parameters.AddWithValue("@Name", permission.PermissionName);
        cmd.ExecuteNonQuery();
    }

    public Permission GetPermission(int id)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        using var cmd = new NpgsqlCommand("SELECT Id, Name FROM Permissions WHERE Id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new Permission { PermissionID = reader.GetInt32(0), PermissionName = reader.GetString(1) };
        }
        return null;
    }

    public void UpdatePermission(Permission permission)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        using var cmd = new NpgsqlCommand("UPDATE Permissions SET Name = @Name WHERE Id = @Id", conn);
        cmd.Parameters.AddWithValue("@Name", permission.PermissionName);
        cmd.Parameters.AddWithValue("@Id", permission.PermissionID);
        cmd.ExecuteNonQuery();
    }

    public void DeletePermission(int id)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        using var cmd = new NpgsqlCommand("DELETE FROM Permissions WHERE Id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.ExecuteNonQuery();
    }

    // User CRUD
    public void CreateUser(User user)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        using var cmd = new NpgsqlCommand("INSERT INTO Users (Username, Password) VALUES (@Username, @Password)", conn);
        cmd.Parameters.AddWithValue("@Username", user.Name);
        cmd.Parameters.AddWithValue("@Password", user.PasswordHash);
        cmd.ExecuteNonQuery();
    }

    public User GetUser(int id)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        using var cmd = new NpgsqlCommand("SELECT Id, Username, Password FROM Users WHERE Id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new User { UserId = reader.GetInt32(0), Name = reader.GetString(1), PasswordHash = reader.GetString(2) };
        }
        return null;
    }

    public void UpdateUser(User user)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        using var cmd = new NpgsqlCommand("UPDATE Users SET Username = @Username, Password = @Password WHERE Id = @Id", conn);
        cmd.Parameters.AddWithValue("@Username", user.Name);
        cmd.Parameters.AddWithValue("@Password", user.PasswordHash);
        cmd.Parameters.AddWithValue("@Id", user.UserId);
        cmd.ExecuteNonQuery();
    }

    public void DeleteUser(int id)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        using var cmd = new NpgsqlCommand("DELETE FROM Users WHERE Id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.ExecuteNonQuery();
    }

    // Role-Permission/User mapping methods
    public void AssignPermissionsToRole(int roleId, IEnumerable<int> permissionIds)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        using var tran = conn.BeginTransaction();
        conn.Execute("DELETE FROM RolePermissionMap WHERE RoleId = @RoleId", new { RoleId = roleId }, tran);
        foreach (var pid in permissionIds)
        {
            conn.Execute("INSERT INTO RolePermissionMap (RoleId, PermissionId) VALUES (@RoleId, @PermissionId)", new { RoleId = roleId, PermissionId = pid }, tran);
        }
        tran.Commit();
    }

    public void AssignUsersToRole(int roleId, IEnumerable<int> userIds)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        using var tran = conn.BeginTransaction();
        conn.Execute("DELETE FROM RoleUserMap WHERE RoleId = @RoleId", new { RoleId = roleId }, tran);
        foreach (var uid in userIds)
        {
            conn.Execute("INSERT INTO RoleUserMap (RoleId, UserId) VALUES (@RoleId, @UserId)", new { RoleId = roleId, UserId = uid }, tran);
        }
        tran.Commit();
    }

    public void AssignRolesToUser(int userId, IEnumerable<int> roleIds)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        using var tran = conn.BeginTransaction();
        conn.Execute("DELETE FROM RoleUserMap WHERE UserId = @UserId", new { UserId = userId }, tran);
        foreach (var rid in roleIds)
        {
            conn.Execute("INSERT INTO RoleUserMap (RoleId, UserId) VALUES (@RoleId, @UserId)", new { RoleId = rid, UserId = userId }, tran);
        }
        tran.Commit();
    }

    public void AddPermissionsToRole(int roleId, IEnumerable<int> permissionIds)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        foreach (var pid in permissionIds)
        {
            conn.Execute("INSERT INTO RolePermissionMap (RoleId, PermissionId) VALUES (@RoleId, @PermissionId) ON CONFLICT DO NOTHING", new { RoleId = roleId, PermissionId = pid });
        }
    }

    public void RemovePermissionsFromRole(int roleId, IEnumerable<int> permissionIds)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        foreach (var pid in permissionIds)
        {
            conn.Execute("DELETE FROM RolePermissionMap WHERE RoleId = @RoleId AND PermissionId = @PermissionId", new { RoleId = roleId, PermissionId = pid });
        }
    }

    public void AddUsersToRole(int roleId, IEnumerable<int> userIds)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        foreach (var uid in userIds)
        {
            conn.Execute("INSERT INTO RoleUserMap (RoleId, UserId) VALUES (@RoleId, @UserId) ON CONFLICT DO NOTHING", new { RoleId = roleId, UserId = uid });
        }
    }

    public void RemoveUsersFromRole(int roleId, IEnumerable<int> userIds)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        foreach (var uid in userIds)
        {
            conn.Execute("DELETE FROM RoleUserMap WHERE RoleId = @RoleId AND UserId = @UserId", new { RoleId = roleId, UserId = uid });
        }
    }

    public void AddRolesToUser(int userId, IEnumerable<int> roleIds)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        foreach (var rid in roleIds)
        {
            conn.Execute("INSERT INTO RoleUserMap (RoleId, UserId) VALUES (@RoleId, @UserId) ON CONFLICT DO NOTHING", new { RoleId = rid, UserId = userId });
        }
    }

    public void RemoveRolesFromUser(int userId, IEnumerable<int> roleIds)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        foreach (var rid in roleIds)
        {
            conn.Execute("DELETE FROM RoleUserMap WHERE RoleId = @RoleId AND UserId = @UserId", new { RoleId = rid, UserId = userId });
        }
    }
}
