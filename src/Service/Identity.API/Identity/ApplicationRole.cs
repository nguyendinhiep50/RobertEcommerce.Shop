namespace Identity.API.Identity;

public class ApplicationRole : IdentityRole<string>
{
	public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();
}
