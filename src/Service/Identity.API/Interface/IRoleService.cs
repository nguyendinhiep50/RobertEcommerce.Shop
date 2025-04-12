namespace Identity.API.Interface;

public interface IRoleService
{
	Task<IEnumerable<ApplicationRole>> GetAllRolesAsync();
	Task<ApplicationRole?> GetRoleByIdAsync(string roleId);
	Task<IdentityResult> CreateRoleAsync(string roleName);
	Task<IdentityResult> UpdateRoleAsync(string roleId, string newRoleName);
	Task<IdentityResult> DeleteRoleAsync(string roleId);
	Task<bool> RoleExistsAsync(string roleName);
}
