using System.ComponentModel.DataAnnotations.Schema;


namespace FrameworkQ.ConsularServices.Users;

[ActionVerb( Verb = "user", PKs = new[] { "UserId" })]
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
    
    [System.ComponentModel.DataAnnotations.Key]
    [Column("user_id")]
    public long UserId { get; set; }

    [MetaData(Title = "Name", IsRequired = true, Description = "Please enter the user's name")]
    [Column("name")]
    public string Name { get; set; }

    [MetaData(Title = "Email", IsRequired = true, Description = "Please enter the user's email address")]
    [Column("email")]
    public string Email { get; set; }

    [MetaData(IsVisible = false)]
    [Column("password_hash")]
    public string PasswordHash { get; set; }
}