using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FrameworkQ.ConsularServices.Users;

public class Role
{
    [Key]
    [Column("role_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long RoleID { get; set; }

    [Column("role_name")]
    public required string RoleName { get; set; }
}