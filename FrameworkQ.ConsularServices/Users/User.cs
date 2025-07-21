namespace FrameworkQ.ConsularServices.Users;

public class User
{
    [Column("user_id")]
    public int UserId { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("email")]
    public string Email { get; set; }

    [Column("password_hash")]
    public string PasswordHash { get; set; }
}