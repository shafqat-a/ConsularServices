using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.X509Certificates;

namespace FrameworkQ.ConsularServices.Users;

public class Permission
{
    [Column("permission_id")]
    public long PermissionID { get; set; }

    [Column("permission_name")]
    public string PermissionName { get; set; }

    public const long UPDATE_USER    = 02;
    public const long DELETE_USER    = 03;
    public const long DISABLE_USER   = 04;
    public const long CHANGE_PASSWORD = 05;

    public const long UPDATE_ROLE    = 12;
    public const long MODIFY_ROLE    = 23;
    public const long DELETE_ROLE    = 14;

    public const long UPDATE_SERVICE_INFO = 22;

    public const long CREATE_TOKEN   = 32;
    public const long UPDATE_TOKEN   = 33;

    public const long UPDATE_SERVICE_INSTANCE = 42;
}