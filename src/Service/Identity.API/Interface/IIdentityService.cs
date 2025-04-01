namespace Identity.API.Interface;

public interface IIdentityService
{
	#region Login
	Task<ResponseModel<UserLoginResponse>> LoginForClient(string userName, string password);
	Task<ResponseModel<UserLoginResponse>> LoginForAdmin(string userName, string password);
	#endregion

	#region Infomation
	Task<ResponseModel<UserDto>> GetInformationUserByAdmin();
	Task<ResponseModel<CustomUserClientDto>> GetInfotmationUserByUser();
	Task<string?> GetUserNameForAdminAsync();
	#endregion

	#region Create Account
	Task<ResponseModel<ApplicationUserDto>> CreateUserForAdminAsync(UserCreateDto userDto);
	Task<ResponseModel<CustomUserClientDto>> CreateUserForClientAsync(CustomUserClientDto userDto);
	#endregion

	#region Delete User
	Task<Result> DeleteUserForClientAsync();
	Task<Result> DeleteUserForAdminAsync();
	#endregion

	#region Update User
	Task<ResponseModel<UserUpdateDto>> UpdateUserForAdminAsync(UserUpdateDto user);
	Task<ResponseModel<UserUpdateDto>> UpdateUserForClientAsync(UserUpdateDto userDto);
	#endregion

	#region Other
	Task<bool> IsInRoleAsync(string userId, string role);
	Task<bool> AuthorizeAsync(string userId, string policyName);
	#endregion

	#region Check Exist Account
	Task<bool> IsExistAccount(string email, string? typeAccount);
	#endregion

	#region Get User EC
	Task<List<ApplicationUserDto>> GetListUserByAdmin();
	#endregion
}
