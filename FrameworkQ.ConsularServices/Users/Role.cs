using System.ComponentModel.DataAnnotations.Schema;

namespace FrameworkQ.ConsularServices.Users;

public class Role
{
    [System.ComponentModel.DataAnnotations.Key]
    [Column("role_id")]
    public long RoleID { get; set; }

    [Column("role_name")]
    public string RoleName { get; set; }
}