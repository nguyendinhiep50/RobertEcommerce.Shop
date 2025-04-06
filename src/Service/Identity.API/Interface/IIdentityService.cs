using System.Linq.Expressions;

namespace Identity.API.Interface;

public interface IIdentityService
{
	#region Get
	IQueryable<ApplicationUser> GetListUserByAdmin(
		Expression<Func<ApplicationUser, bool>>? filterExpression = null,
		List<(Expression<Func<ApplicationUser, object>>, bool)>? sortExpressions = null,
		int take = 10);

	IQueryable<ApplicationUser> GetUserByAdmin(string userId);

	IQueryable<ApplicationUser> GetUserByClient(string userId);

	IQueryable<ApplicationUser> GetUserByToken(string token);

	Task<ApplicationUser?> GetByEmailAsync(string email);

	Task<IList<string>> GetRolesAsync(ApplicationUser user);
	#endregion

	#region Create
	Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password, IEnumerable<string>? roles = null);

	Task<bool> CreateUserWithEmailConfirmationAsync(ApplicationUser user, string password);
	#endregion

	#region Update
	Task<IdentityResult> UpdateUserAsync(ApplicationUser user);

	Task<bool> UpdatePasswordAsync(string userId, string currentPassword, string newPassword);
	#endregion

	#region Delete
	Task<IdentityResult> DeleteUserAsync(string userId);

	#endregion

	#region Authentication
	Task<string?> AuthenticateAsync(string email, string password);

	Task<bool> ValidateTokenAsync(string token);
	#endregion

	#region Token
	Task<string> GenerateJwtTokenAsync(ApplicationUser user);

	Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user);

	Task<IdentityResult> ConfirmEmailAsync(string userId, string token);

	Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);

	Task<IdentityResult> ResetPasswordAsync(string email, string token, string newPassword);
	#endregion

	#region Role Management
	Task<IdentityResult> AddToRolesAsync(ApplicationUser user, IEnumerable<string> roles);

	Task<IdentityResult> RemoveFromRolesAsync(ApplicationUser user, IEnumerable<string> roles);

	Task<bool> IsInRoleAsync(ApplicationUser user, string role);
	#endregion

	#region Utilities
	string? GetUserIdFromClaims(ClaimsPrincipal user);

	string? GetUserEmailFromClaims(ClaimsPrincipal user);
	#endregion
}