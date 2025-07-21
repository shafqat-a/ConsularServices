using System.Security.Cryptography.X509Certificates;

namespace FrameworkQ.ConsularServices.Users;

public class Permission
{
    [Column("permission_id")]
    public int PermissionID { get; set; }

    [Column("permission_name")]
    public string PermissionName { get; set; }
}