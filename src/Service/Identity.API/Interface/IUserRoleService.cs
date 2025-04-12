namespace Identity.API.Interface;

public interface IUserRoleService
{
	Task<IList<string>> GetUserRolesAsync(string userId);
	Task<IdentityResult> AddUserToRoleAsync(string userId, string roleName);
	Task<IdentityResult> RemoveUserFromRoleAsync(string userId, string roleName);
	Task<bool> IsUserInRoleAsync(string userId, string roleName);
	Task<IdentityResult> ReplaceUserRolesAsync(string userId, IEnumerable<string> newRoles);
}
