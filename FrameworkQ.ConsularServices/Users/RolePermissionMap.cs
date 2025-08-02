using System.ComponentModel.DataAnnotations.Schema;

namespace FrameworkQ.ConsularServices.Users;

public class RolePermissionMap
{
    [Column("role_id")]
    public int RoleId { get; set; }

    [Column("permission_id")]
    public int PermissionId { get; set; }
}