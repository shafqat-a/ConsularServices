using Microsoft.EntityFrameworkCore;
using FrameworkQ.ConsularServices.Data;
using FrameworkQ.ConsularServices.Users;

namespace FrameworkQ.ConsularServices.Users
{
    public class UserRepositoryEF : IUserRepository
    {
        private readonly ConsularDbContext _context;

        public UserRepositoryEF(ConsularDbContext context)
        {
            _context = context;
        }

        // ---------- Role CRUD ----------
        public Role CreateRole(Role role)
        {
            _context.Roles.Add(role);
            _context.SaveChanges();
            return role;
        }

        public Role GetRole(long id)
        {
            return _context.Roles.Find(id)!;
        }

        public Role GetRoleByName(string roleName)
        {
            return _context.Roles.FirstOrDefault(r => r.RoleName == roleName)!;
        }

        public void UpdateRole(Role role)
        {
            _context.Roles.Update(role);
            _context.SaveChanges();
        }

        public void DeleteRole(long id)
        {
            var role = _context.Roles.Find(id);
            if (role != null)
            {
                _context.Roles.Remove(role);
                _context.SaveChanges();
            }
        }

        // ---------- Permission CRUD ----------
        public Permission CreatePermission(Permission permission)
        {
            _context.Permissions.Add(permission);
            _context.SaveChanges();
            return permission;
        }

        public Permission GetPermission(long id)
        {
            return _context.Permissions.Find(id)!;
        }

        public List<Permission> GetAllPermissions()
        {
            return _context.Permissions.ToList();
        }

        public void UpdatePermission(Permission permission)
        {
            _context.Permissions.Update(permission);
            _context.SaveChanges();
        }

        public void DeletePermission(long id)
        {
            var permission = _context.Permissions.Find(id);
            if (permission != null)
            {
                _context.Permissions.Remove(permission);
                _context.SaveChanges();
            }
        }

        // ---------- User CRUD ----------
        public User CreateUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }

        public User GetUser(long id)
        {
            return _context.Users.Find(id)!;
        }

        public User GetUserByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email)!;
        }

        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void DeleteUser(long id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }

        public User[] GetUsers()
        {
            return _context.Users.ToArray();
        }

        // ---------- Role-Permission / User mapping methods ----------
        public void AssignPermissionsToRole(long roleId, IEnumerable<long> permissionIds)
        {
            // Remove existing permissions for this role
            var existingMappings = _context.RolePermissionMaps.Where(rpm => rpm.RoleId == roleId);
            _context.RolePermissionMaps.RemoveRange(existingMappings);

            // Add new permissions
            foreach (var permissionId in permissionIds)
            {
                _context.RolePermissionMaps.Add(new RolePermissionMap
                {
                    RoleId = roleId,
                    PermissionId = permissionId
                });
            }
            _context.SaveChanges();
        }

        public void AssignUsersToRole(long roleId, IEnumerable<long> userIds)
        {
            // Remove existing users for this role
            var existingMappings = _context.RoleUserMaps.Where(rum => rum.RoleId == roleId);
            _context.RoleUserMaps.RemoveRange(existingMappings);

            // Add new users
            foreach (var userId in userIds)
            {
                _context.RoleUserMaps.Add(new RoleUserMap
                {
                    UserId = userId,
                    RoleId = roleId
                });
            }
            _context.SaveChanges();
        }

        public void AssignRolesToUser(long userId, IEnumerable<long> roleIds)
        {
            // Remove existing roles for this user
            var existingMappings = _context.RoleUserMaps.Where(rum => rum.UserId == userId);
            _context.RoleUserMaps.RemoveRange(existingMappings);

            // Add new roles
            foreach (var roleId in roleIds)
            {
                _context.RoleUserMaps.Add(new RoleUserMap
                {
                    UserId = userId,
                    RoleId = roleId
                });
            }
            _context.SaveChanges();
        }

        public void AddPermissionsToRole(long roleId, IEnumerable<long> permissionIds)
        {
            foreach (var permissionId in permissionIds)
            {
                var exists = _context.RolePermissionMaps.Any(rpm => 
                    rpm.RoleId == roleId && rpm.PermissionId == permissionId);
                
                if (!exists)
                {
                    _context.RolePermissionMaps.Add(new RolePermissionMap
                    {
                        RoleId = roleId,
                        PermissionId = permissionId
                    });
                }
            }
            _context.SaveChanges();
        }

        public void RemovePermissionsFromRole(long roleId, IEnumerable<long> permissionIds)
        {
            var mappingsToRemove = _context.RolePermissionMaps.Where(rpm => 
                rpm.RoleId == roleId && permissionIds.Contains(rpm.PermissionId));
            
            _context.RolePermissionMaps.RemoveRange(mappingsToRemove);
            _context.SaveChanges();
        }

        public void AddUsersToRole(long roleId, IEnumerable<long> userIds)
        {
            foreach (var userId in userIds)
            {
                var exists = _context.RoleUserMaps.Any(rum => 
                    rum.RoleId == roleId && rum.UserId == userId);
                
                if (!exists)
                {
                    _context.RoleUserMaps.Add(new RoleUserMap
                    {
                        UserId = userId,
                        RoleId = roleId
                    });
                }
            }
            _context.SaveChanges();
        }

        public void RemoveUsersFromRole(long roleId, IEnumerable<long> userIds)
        {
            var mappingsToRemove = _context.RoleUserMaps.Where(rum => 
                rum.RoleId == roleId && userIds.Contains(rum.UserId));
            
            _context.RoleUserMaps.RemoveRange(mappingsToRemove);
            _context.SaveChanges();
        }

        public void AddRolesToUser(long userId, IEnumerable<long> roleIds)
        {
            foreach (var roleId in roleIds)
            {
                var exists = _context.RoleUserMaps.Any(rum => 
                    rum.UserId == userId && rum.RoleId == roleId);
                
                if (!exists)
                {
                    _context.RoleUserMaps.Add(new RoleUserMap
                    {
                        UserId = userId,
                        RoleId = roleId
                    });
                }
            }
            _context.SaveChanges();
        }

        public void RemoveRolesFromUser(long userId, IEnumerable<long> roleIds)
        {
            var mappingsToRemove = _context.RoleUserMaps.Where(rum => 
                rum.UserId == userId && roleIds.Contains(rum.RoleId));
            
            _context.RoleUserMaps.RemoveRange(mappingsToRemove);
            _context.SaveChanges();
        }

        public bool HasPermission(long userId, long permissionId)
        {
            return _context.RoleUserMaps
                .Join(_context.RolePermissionMaps,
                    rum => rum.RoleId,
                    rpm => rpm.RoleId,
                    (rum, rpm) => new { rum.UserId, rpm.PermissionId })
                .Any(x => x.UserId == userId && x.PermissionId == permissionId);
        }

        public bool HasPermission(long userId, string permissionName)
        {
            return _context.RoleUserMaps
                .Join(_context.RolePermissionMaps,
                    rum => rum.RoleId,
                    rpm => rpm.RoleId,
                    (rum, rpm) => new { rum.UserId, rpm.PermissionId })
                .Join(_context.Permissions,
                    x => x.PermissionId,
                    p => p.PermissionID,
                    (x, p) => new { x.UserId, p.PermissionName })
                .Any(x => x.UserId == userId && x.PermissionName == permissionName);
        }
    }
}
