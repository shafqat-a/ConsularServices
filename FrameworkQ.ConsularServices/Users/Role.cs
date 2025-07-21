namespace FrameworkQ.ConsularServices.Users;

public class Role
{
    [Column("role_id")]
    public int RoleID { get; set; }

    [Column("role_name")]
    public string RoleName { get; set; }
}