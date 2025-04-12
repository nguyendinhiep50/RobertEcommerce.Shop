namespace Identity.API.Identity;

public class ApplicationRole : IdentityRole<string>
{
	public ApplicationRole() : base() { }

	public ApplicationRole(string roleName) : base(roleName) { }

	public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();
}
