namespace FrameworkQ.ConsularServices.Users;

public class RoleUserMap
{
    [Column("user_id")]
    public int UserId { get; set; }

    [Column("role_id")]
    public int RoleId { get; set; }
}
