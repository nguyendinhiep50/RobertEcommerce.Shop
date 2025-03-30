namespace Identity.API.Interface;

public interface IUser
{
	string? Id { get; }
	string? DeviceType { get; }
	IEnumerable<string> GetUserRoles();
	string GetWebHostEnvironment();
	bool IsRoleAdmin();
}
