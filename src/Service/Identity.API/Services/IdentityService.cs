using AutoMapper;
using Identity.API.Identity.OverrideIdentity;
using System.Linq.Expressions;

namespace Identity.API.Services;

public class IdentityService : IIdentityService
{
	private readonly CustomUserManager _userManager;
	private readonly SignInManager<ApplicationUser> _signInManager;
	private readonly IMapper _mapper;

	public IdentityService(
		CustomUserManager userManager,
		SignInManager<ApplicationUser> signInManager,
		IMapper mapper)
	{
		_userManager = userManager;
		_signInManager = signInManager;
		_mapper = mapper;
	}

	#region Get
	public IQueryable<ApplicationUser> GetListUserByAdmin(
		Expression<Func<ApplicationUser, bool>>? filterExpression = null,
		List<(Expression<Func<ApplicationUser, object>>, bool)>? sortExpressions = null,
		int take = 10)
	{
		var query = _userManager.Users.AsQueryable();

		if (filterExpression != null)
		{
			query = query.Where(filterExpression);
		}

		if (sortExpressions != null && sortExpressions.Any())
		{
			var firstSort = sortExpressions.First();
			var orderedQuery = firstSort.Item2
				? query.OrderByDescending(firstSort.Item1)
				: query.OrderBy(firstSort.Item1);

			foreach (var sort in sortExpressions.Skip(1))
			{
				orderedQuery = sort.Item2
					? orderedQuery.ThenByDescending(sort.Item1)
					: orderedQuery.ThenBy(sort.Item1);
			}

			query = orderedQuery;
		}

		return query.Take(take);
	}

	public IQueryable<ApplicationUser> GetUserByAdmin(string userId)
	{
		return _userManager.Users.Where(u => u.Id == userId);
	}

	public IQueryable<ApplicationUser> GetUserByClient(string userId)
	{
		return _userManager.Users.Where(u => u.Id == userId);
	}

	public IQueryable<ApplicationUser> GetUserByToken(string token)
	{
		var userId = Identity.IdentityUtility.GetUserIdFromToken(token);
		return userId != null ? _userManager.Users.Where(u => u.Id == userId) : Enumerable.Empty<ApplicationUser>().AsQueryable();
	}

	public async Task<ApplicationUser?> GetByEmailAsync(string email)
	{
		return await _userManager.FindByEmailAsync(email);
	}

	public async Task<IList<string>> GetRolesAsync(ApplicationUser user)
	{
		return await _userManager.GetRolesAsync(user);
	}
	#endregion

	#region Create
	public async Task<IdentityResult> CreateUserAsync(
		UserCreateDto userDto,
		string password,
		IEnumerable<string>? roles = null)
	{
		var user = _mapper.Map<ApplicationUser>(userDto);

		var lastUser = await _userManager.Users
			.OrderByDescending(u => u.CreatedDate)
			.FirstOrDefaultAsync();

		if (lastUser == null) return null!;

		user.Id = IdFormatter.GenerateSequentialIdByDate(
			lastUser.Id,
			Constant.CONSTANT_KEY_USER_ID);

		var result = await _userManager.CreateAsync(user, password);

		if (result.Succeeded && roles != null)
		{
			await _userManager.AddToRolesAsync(user, roles);
		}

		return result;
	}

	public async Task<bool> CreateUserWithEmailConfirmationAsync(ApplicationUser user, string password)
	{
		var result = await _userManager.CreateAsync(user, password);
		if (!result.Succeeded) return false;

		var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
		return true;
	}
	#endregion

	#region Update
	public async Task<IdentityResult> UpdateUserAsync(ApplicationUser user)
	{
		return await _userManager.UpdateAsync(user);
	}

	public async Task<bool> UpdatePasswordAsync(string userId, string currentPassword, string newPassword)
	{
		var user = await _userManager.FindByIdAsync(userId);
		if (user == null) return false;

		var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
		return result.Succeeded;
	}
	#endregion

	#region Delete
	public async Task<IdentityResult> DeleteUserAsync(string userId)
	{
		var user = await _userManager.FindByIdAsync(userId);
		return user != null ? await _userManager.DeleteAsync(user) : IdentityResult.Failed();
	}
	#endregion

	#region Authentication
	public async Task<string?> AuthenticateAsync(string email, string password)
	{
		var user = await _userManager.FindByEmailAsync(email);
		if (user == null) return null;

		var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
		if (!result.Succeeded) return null;

		return await GenerateJwtTokenAsync(user);
	}

	public Task<bool> ValidateTokenAsync(string token)
	{
		return Task.FromResult(!string.IsNullOrEmpty(Identity.IdentityUtility.GetUserIdFromToken(token)));
	}
	#endregion

	#region Token
	public async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
	{
		return  Identity.IdentityUtility.GenerateToken(
			user,
			JwtSettings.SecretKey,
			user.Id,
			user.Name,
			null);
	}

	public async Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user)
	{
		return await _userManager.GenerateEmailConfirmationTokenAsync(user);
	}

	public async Task<IdentityResult> ConfirmEmailAsync(string userId, string token)
	{
		var user = await _userManager.FindByIdAsync(userId);
		return user != null ? await _userManager.ConfirmEmailAsync(user, token) : IdentityResult.Failed();
	}

	public async Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user)
	{
		return await _userManager.GeneratePasswordResetTokenAsync(user);
	}

	public async Task<IdentityResult> ResetPasswordAsync(string email, string token, string newPassword)
	{
		var user = await _userManager.FindByEmailAsync(email);
		return user != null ? await _userManager.ResetPasswordAsync(user, token, newPassword) : IdentityResult.Failed();
	}
	#endregion

	#region Role Management
	public async Task<IdentityResult> AddToRolesAsync(ApplicationUser user, IEnumerable<string> roles)
	{
		return await _userManager.AddToRolesAsync(user, roles);
	}

	public async Task<IdentityResult> RemoveFromRolesAsync(ApplicationUser user, IEnumerable<string> roles)
	{
		return await _userManager.RemoveFromRolesAsync(user, roles);
	}

	public async Task<bool> IsInRoleAsync(ApplicationUser user, string role)
	{
		return await _userManager.IsInRoleAsync(user, role);
	}
	#endregion

	#region Utilities
	public string? GetUserIdFromClaims(ClaimsPrincipal user)
	{
		return string.Empty;
	}

	public string? GetUserEmailFromClaims(ClaimsPrincipal user)
	{
		return user.FindFirst(ClaimTypes.Email)?.Value;
	}

	#endregion
}

