namespace Identity.API.Services;

public class RoleService : IRoleService
{
	private readonly RoleManager<ApplicationRole> _roleManager;

	public RoleService(RoleManager<ApplicationRole> roleManager)
	{
		_roleManager = roleManager;
	}

	public async Task<IEnumerable<ApplicationRole>> GetAllRolesAsync()
	{
		return await _roleManager.Roles.ToListAsync();
	}

	public async Task<ApplicationRole?> GetRoleByIdAsync(string roleId)
	{
		return await _roleManager.FindByIdAsync(roleId);
	}

	public async Task<IdentityResult> CreateRoleAsync(string roleName)
	{
		if (await _roleManager.RoleExistsAsync(roleName))
			return IdentityResult.Failed(new IdentityError { Description = "Role already exists." });

		return await _roleManager.CreateAsync(new ApplicationRole(roleName));
	}

	public async Task<IdentityResult> UpdateRoleAsync(string roleId, string newRoleName)
	{
		var role = await _roleManager.FindByIdAsync(roleId);
		if (role == null)
			return IdentityResult.Failed(new IdentityError { Description = "Role not found." });

		role.Name = newRoleName;
		return await _roleManager.UpdateAsync(role);
	}

	public async Task<IdentityResult> DeleteRoleAsync(string roleId)
	{
		var role = await _roleManager.FindByIdAsync(roleId);
		if (role == null)
			return IdentityResult.Failed(new IdentityError { Description = "Role not found." });

		return await _roleManager.DeleteAsync(role);
	}

	public async Task<bool> RoleExistsAsync(string roleName)
	{
		return await _roleManager.RoleExistsAsync(roleName);
	}
}
