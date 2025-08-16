using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace FrameworkQ.ConsularServices.Users;

[EntityMeta(UrlStem = "user", Verb = "user", PKs = new[] { "UserId" })]
public class User
{
    /*
    case "user":
            obj.title = "User";
            obj.elements = [
                { type: "text", name: "email", title: "Please type in your email", isRequired: true },
                { type: "text", name: "password", title: "Please enter your password", isRequired: true, inputType: "password" },
                { type: "text", name: "userId", visible: false },
                { type: "text", name: "passwordHash", visible: false },
            ]; 
            break;
            */
    [PropertyMeta(IsVisible =false)]
    [Key]
    [Column("user_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long UserId { get; set; }

    [PropertyMeta(Title = "Name", IsRequired = true, Description = "Please enter the user's name")]
    [Column("name")]
    public required string Name { get; set; }

    [PropertyMeta(Title = "Email", IsRequired = true, Description = "Please enter the user's email address")]
    [Column("email")]
    public required string Email { get; set; }

    [PropertyMeta(IsVisible = false)]
    [Column("password_hash")]
    public required string PasswordHash { get; set; }
}