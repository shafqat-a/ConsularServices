using System.ComponentModel.DataAnnotations.Schema;

namespace FrameworkQ.ConsularServices.Users;

public class RolePermissionMap
{
    
    [Column("role_id")]
    public long RoleId { get; set; }

    [Column("permission_id")]
    public long PermissionId { get; set; }
}