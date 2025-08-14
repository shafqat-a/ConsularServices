using System.ComponentModel.DataAnnotations.Schema;

namespace FrameworkQ.ConsularServices.Users;

public class RoleUserMap
{
    [Column("user_id")]
    public long UserId { get; set; }

    [Column("role_id")]
    public long RoleId { get; set; }
}
