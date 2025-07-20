using FrameworkQ.ConsularServices.Users;

namespace FrameworkQ.ConsularServices;
public interface IUserRepository
{
    void CreateRole(Role role);
    Role GetRole(int id);
    void UpdateRole(Role role);
    void DeleteRole(int id);

    void CreatePermission(Permission permission);
    Permission GetPermission(int id);
    void UpdatePermission(Permission permission);
    void DeletePermission(int id);

    void CreateUser(User user);
    User GetUser(int id);
    void UpdateUser(User user);
    void DeleteUser(int id);

    void AssignPermissionsToRole(int roleId, IEnumerable<int> permissionIds);
    void AssignUsersToRole(int roleId, IEnumerable<int> userIds);
    void AssignRolesToUser(int userId, IEnumerable<int> roleIds);
    void AddPermissionsToRole(int roleId, IEnumerable<int> permissionIds);
    void RemovePermissionsFromRole(int roleId, IEnumerable<int> permissionIds);
    void AddUsersToRole(int roleId, IEnumerable<int> userIds);
    void RemoveUsersFromRole(int roleId, IEnumerable<int> userIds);
    void AddRolesToUser(int userId, IEnumerable<int> roleIds);
    void RemoveRolesFromUser(int userId, IEnumerable<int> roleIds);

    User GetUserByEmail(string email);
}