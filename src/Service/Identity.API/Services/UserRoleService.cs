namespace Identity.API.Services;

public class UserRoleService : IUserRoleService
{
	private readonly UserManager<ApplicationUser> _userManager;

	public UserRoleService(UserManager<ApplicationUser> userManager)
	{
		_userManager = userManager;
	}

	public async Task<IList<string>> GetUserRolesAsync(string userId)
	{
		var user = await _userManager.FindByIdAsync(userId);
		if (user == null)
			throw new Exception("User not found");

		return await _userManager.GetRolesAsync(user);
	}

	public async Task<IdentityResult> AddUserToRoleAsync(string userId, string roleName)
	{
		var user = await _userManager.FindByIdAsync(userId);
		if (user == null)
			return IdentityResult.Failed(new IdentityError { Description = "User not found" });

		return await _userManager.AddToRoleAsync(user, roleName);
	}

	public async Task<IdentityResult> RemoveUserFromRoleAsync(string userId, string roleName)
	{
		var user = await _userManager.FindByIdAsync(userId);
		if (user == null)
			return IdentityResult.Failed(new IdentityError { Description = "User not found" });

		return await _userManager.RemoveFromRoleAsync(user, roleName);
	}

	public async Task<bool> IsUserInRoleAsync(string userId, string roleName)
	{
		var user = await _userManager.FindByIdAsync(userId);
		if (user == null)
			return false;

		return await _userManager.IsInRoleAsync(user, roleName);
	}

	public async Task<IdentityResult> ReplaceUserRolesAsync(string userId, IEnumerable<string> newRoles)
	{
		var user = await _userManager.FindByIdAsync(userId);
		if (user == null)
			return IdentityResult.Failed(new IdentityError { Description = "User not found" });

		var currentRoles = await _userManager.GetRolesAsync(user);
		var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
		if (!removeResult.Succeeded) return removeResult;

		return await _userManager.AddToRolesAsync(user, newRoles);
	}
}
