namespace Identity.API.Services;

public class CurrentUser : IUser
{
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IWebHostEnvironment _webHostEnvironment;

	public CurrentUser(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment)
	{
		_httpContextAccessor = httpContextAccessor;
		_webHostEnvironment = webHostEnvironment;
	}

	/// <summary>
	/// Retrieves the current user's ID.
	/// </summary>
	/// <returns>User ID as a string, or null if the user is not authenticated.</returns>
	public string? Id => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

	/// <summary>
	/// Retrieves the current user's username.
	/// </summary>
	/// <returns>Username as a string, or null if the user is not authenticated.</returns>
	public string? GetUserName()
	{
		return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);
	}

	/// <summary>
	/// Retrieves the current user's username.
	/// </summary>
	/// <returns>Username as a string, or null if the user is not authenticated.</returns>
	public string? GetWebHostEnvironment()
	{
		return _webHostEnvironment.WebRootPath;
	}

	/// <summary>
	/// Retrieves the roles of the current user.
	/// </summary>
	/// <returns>A list of roles as strings, or an empty list if no roles are found.</returns>
	public IEnumerable<string> GetUserRoles()
	{
		return _httpContextAccessor.HttpContext?.User?.Claims
			.Where(c => c.Type == ClaimTypes.Role)
			.Select(c => c.Value) ?? Enumerable.Empty<string>();
	}

	/// <summary>
	/// Checks if the current user has a specific role.
	/// </summary>
	/// <param name="role">The role to check.</param>
	/// <returns>True if the user has the role, otherwise false.</returns>
	public bool HasRole(string role)
	{
		return _httpContextAccessor.HttpContext?.User?.Claims
			.Any(c => c.Type == ClaimTypes.Role && c.Value.Equals(role, StringComparison.OrdinalIgnoreCase)) ?? false;
	}

	/// <summary>
	/// Checks if the current user has the Admin role.
	/// </summary>
	/// <returns>True if the user has the Admin role, otherwise false.</returns>
	public bool IsRoleAdmin()
	{
		return HasRole(Roles.Administrator);  // Kiểm tra xem người dùng có vai trò Admin hay không
	}

	/// <summary>
	/// Retrieves the device type (e.g., Smartphone, PC, Unknown).
	/// </summary>
	public string DeviceType
	{
		get
		{
			var deviceType = _httpContextAccessor.HttpContext?.Items["DeviceType"];
			return deviceType?.ToString() ?? "Unknown";
		}
	}
}
