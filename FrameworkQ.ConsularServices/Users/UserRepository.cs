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

   // ---------- Role CRUD ----------
public void CreateRole(Role role)
{
    using var conn = new NpgsqlConnection(_connectionString);
    conn.Open();
    conn.Execute("INSERT INTO role (role_name) VALUES (@RoleName)", role);
}

public Role GetRole(int id)
{
    using var conn = new NpgsqlConnection(_connectionString);
    conn.Open();
    return conn.QueryFirstOrDefault<Role>(
        "SELECT role_id AS RoleID, role_name AS RoleName FROM role WHERE role_id = @Id",
        new { Id = id });
}

public void UpdateRole(Role role)
{
    using var conn = new NpgsqlConnection(_connectionString);
    conn.Open();
    conn.Execute("UPDATE role SET role_name = @RoleName WHERE role_id = @RoleID", role);
}

public void DeleteRole(int id)
{
    using var conn = new NpgsqlConnection(_connectionString);
    conn.Open();
    using var cmd = new NpgsqlCommand("DELETE FROM role WHERE role_id = @Id", conn);
    cmd.Parameters.AddWithValue("@Id", id);
    cmd.ExecuteNonQuery();
}

// ---------- Permission CRUD ----------
public void CreatePermission(Permission permission)
{
    using var conn = new NpgsqlConnection(_connectionString);
    conn.Open();
    using var cmd = new NpgsqlCommand("INSERT INTO permission (permission_name) VALUES (@PermissionName)", conn);
    cmd.Parameters.AddWithValue("@PermissionName", permission.PermissionName);
    cmd.ExecuteNonQuery();
}

public Permission GetPermission(int id)
{
    using var conn = new NpgsqlConnection(_connectionString);
    conn.Open();
    using var cmd = new NpgsqlCommand(
        "SELECT permission_id AS PermissionID, permission_name AS PermissionName FROM permission WHERE permission_id = @Id", conn);
    cmd.Parameters.AddWithValue("@Id", id);
    using var reader = cmd.ExecuteReader();
    if (reader.Read())
    {
        return new Permission
        {
            PermissionID = reader.GetInt32(0),
            PermissionName = reader.GetString(1)
        };
    }
    return null;
}

public void UpdatePermission(Permission permission)
{
    using var conn = new NpgsqlConnection(_connectionString);
    conn.Open();
    using var cmd = new NpgsqlCommand("UPDATE permission SET permission_name = @PermissionName WHERE permission_id = @PermissionID", conn);
    cmd.Parameters.AddWithValue("@PermissionName", permission.PermissionName);
    cmd.Parameters.AddWithValue("@PermissionID", permission.PermissionID);
    cmd.ExecuteNonQuery();
}

public void DeletePermission(int id)
{
    using var conn = new NpgsqlConnection(_connectionString);
    conn.Open();
    using var cmd = new NpgsqlCommand("DELETE FROM permission WHERE permission_id = @Id", conn);
    cmd.Parameters.AddWithValue("@Id", id);
    cmd.ExecuteNonQuery();
}

// ---------- User CRUD ----------
public void CreateUser(User user)
{
    using var conn = new NpgsqlConnection(_connectionString);
    conn.Open();
    using var cmd = new NpgsqlCommand(
        "INSERT INTO \"user\" (name, email, password_hash) VALUES (@Name, @Email, @PasswordHash)", conn);
    cmd.Parameters.AddWithValue("@Name", user.Name);
    cmd.Parameters.AddWithValue("@Email", user.Email);
    cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
    cmd.ExecuteNonQuery();
}

public User GetUser(int id)
{
    using var conn = new NpgsqlConnection(_connectionString);
    conn.Open();
    using var cmd = new NpgsqlCommand(
        "SELECT user_id AS UserId, name AS Name, email AS Email, password_hash AS PasswordHash FROM \"user\" WHERE user_id = @Id", conn);
    cmd.Parameters.AddWithValue("@Id", id);
    using var reader = cmd.ExecuteReader();
    if (reader.Read())
    {
        return new User
        {
            UserId = reader.GetInt32(0),
            Name = reader.GetString(1),
            Email = reader.GetString(2),
            PasswordHash = reader.GetString(3)
        };
    }
    return null;
}

public User GetUserByEmail(string email)
{
    using var conn = new NpgsqlConnection(_connectionString);
    const string sql = @"
        SELECT user_id   ,
               name      AS Name,
               password_hash AS PasswordHash
        FROM   ""user""
        WHERE  email = @Email";
    return conn.QueryFirstOrDefault<User>(sql, new { Email = email });
}

public void UpdateUser(User user)
{
    using var conn = new NpgsqlConnection(_connectionString);
    conn.Open();
    using var cmd = new NpgsqlCommand(
        "UPDATE \"user\" SET name = @Name, email = @Email, password_hash = @PasswordHash WHERE user_id = @UserId", conn);
    cmd.Parameters.AddWithValue("@Name", user.Name);
    cmd.Parameters.AddWithValue("@Email", user.Email);
    cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
    cmd.Parameters.AddWithValue("@UserId", user.UserId);
    cmd.ExecuteNonQuery();
}

public void DeleteUser(int id)
{
    using var conn = new NpgsqlConnection(_connectionString);
    conn.Open();
    using var cmd = new NpgsqlCommand("DELETE FROM \"user\" WHERE user_id = @Id", conn);
    cmd.Parameters.AddWithValue("@Id", id);
    cmd.ExecuteNonQuery();
}

// ---------- Role-Permission / User mapping methods ----------
public void AssignPermissionsToRole(int roleId, IEnumerable<int> permissionIds)
{
    using var conn = new NpgsqlConnection(_connectionString);
    conn.Open();
    using var tran = conn.BeginTransaction();
    conn.Execute("DELETE FROM role_permission_map WHERE role_id = @RoleId", new { RoleId = roleId }, tran);
    foreach (var pid in permissionIds)
    {
        conn.Execute("INSERT INTO role_permission_map (role_id, permission_id) VALUES (@RoleId, @PermissionId)",
                     new { RoleId = roleId, PermissionId = pid }, tran);
    }
    tran.Commit();
}

public void AssignUsersToRole(int roleId, IEnumerable<int> userIds)
{
    using var conn = new NpgsqlConnection(_connectionString);
    conn.Open();
    using var tran = conn.BeginTransaction();
    conn.Execute("DELETE FROM role_user_map WHERE role_id = @RoleId", new { RoleId = roleId }, tran);
    foreach (var uid in userIds)
    {
        conn.Execute("INSERT INTO role_user_map (role_id, user_id) VALUES (@RoleId, @UserId)",
                     new { RoleId = roleId, UserId = uid }, tran);
    }
    tran.Commit();
}

public void AssignRolesToUser(int userId, IEnumerable<int> roleIds)
{
    using var conn = new NpgsqlConnection(_connectionString);
    conn.Open();
    using var tran = conn.BeginTransaction();
    conn.Execute("DELETE FROM role_user_map WHERE user_id = @UserId", new { UserId = userId }, tran);
    foreach (var rid in roleIds)
    {
        conn.Execute("INSERT INTO role_user_map (role_id, user_id) VALUES (@RoleId, @UserId)",
                     new { RoleId = rid, UserId = userId }, tran);
    }
    tran.Commit();
}

public void AddPermissionsToRole(int roleId, IEnumerable<int> permissionIds)
{
    using var conn = new NpgsqlConnection(_connectionString);
    conn.Open();
    foreach (var pid in permissionIds)
    {
        conn.Execute("INSERT INTO role_permission_map (role_id, permission_id) VALUES (@RoleId, @PermissionId) ON CONFLICT DO NOTHING",
                     new { RoleId = roleId, PermissionId = pid });
    }
}

public void RemovePermissionsFromRole(int roleId, IEnumerable<int> permissionIds)
{
    using var conn = new NpgsqlConnection(_connectionString);
    conn.Open();
    foreach (var pid in permissionIds)
    {
        conn.Execute("DELETE FROM role_permission_map WHERE role_id = @RoleId AND permission_id = @PermissionId",
                     new { RoleId = roleId, PermissionId = pid });
    }
}

public void AddUsersToRole(int roleId, IEnumerable<int> userIds)
{
    using var conn = new NpgsqlConnection(_connectionString);
    conn.Open();
    foreach (var uid in userIds)
    {
        conn.Execute("INSERT INTO role_user_map (role_id, user_id) VALUES (@RoleId, @UserId) ON CONFLICT DO NOTHING",
                     new { RoleId = roleId, UserId = uid });
    }
}

public void RemoveUsersFromRole(int roleId, IEnumerable<int> userIds)
{
    using var conn = new NpgsqlConnection(_connectionString);
    conn.Open();
    foreach (var uid in userIds)
    {
        conn.Execute("DELETE FROM role_user_map WHERE role_id = @RoleId AND user_id = @UserId",
                     new { RoleId = roleId, UserId = uid });
    }
}

public void AddRolesToUser(int userId, IEnumerable<int> roleIds)
{
    using var conn = new NpgsqlConnection(_connectionString);
    conn.Open();
    foreach (var rid in roleIds)
    {
        conn.Execute("INSERT INTO role_user_map (role_id, user_id) VALUES (@RoleId, @UserId) ON CONFLICT DO NOTHING",
                     new { RoleId = rid, UserId = userId });
    }
}

public void RemoveRolesFromUser(int userId, IEnumerable<int> roleIds)
{
    using var conn = new NpgsqlConnection(_connectionString);
    conn.Open();
    foreach (var rid in roleIds)
    {
        conn.Execute("DELETE FROM role_user_map WHERE role_id = @RoleId AND user_id = @UserId",
                     new { RoleId = rid, UserId = userId });
    }
}
}
