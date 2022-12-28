
using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{


    public virtual ICollection<Post>? posts { get; set; }

}
