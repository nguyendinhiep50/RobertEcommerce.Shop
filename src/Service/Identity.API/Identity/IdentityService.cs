using AutoMapper;
using Identity.API.Identities.Dtos;
using Identity.API.Identities.Users;
using Identity.API.Identity.OverrideIdentity;
using Identity.API.Interface;
using Microsoft.AspNetCore.Authorization;

namespace Identity.API.Identity;

public class IdentityService : IIdentityService
{
	private readonly IConfiguration _configuration;
	private readonly CustomUserManager _userManager;
	private readonly CustomUserClient _userClient;
	private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
	private readonly IAuthorizationService _authorizationService;
	private readonly SignInManager<ApplicationUser> _signInManager;
	private readonly SignInManager<Rb_CustomerUser> _signInManagerCustomer;
	private readonly IMapper _mapper;
	private readonly IUser _user;

	public IdentityService(
		IConfiguration configuration,
		CustomUserManager userManager,
		CustomUserClient userClient,
		SignInManager<ApplicationUser> signInManager,
		SignInManager<Rb_CustomerUser> signInManagerCustomer,
		IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
		IMapper mapper,
		IAuthorizationService authorizationService,
		IUser user)
	{
		_configuration = configuration;
		_userManager = userManager;
		_userClient = userClient;
		_userClaimsPrincipalFactory = userClaimsPrincipalFactory;
		_authorizationService = authorizationService;
		_signInManager = signInManager;
		_signInManagerCustomer = signInManagerCustomer;
		_mapper = mapper;
		_user = user;
	}

	#region Login
	public async Task<ResponseModel<UserLoginResponse>> LoginForClient(string userName, string password)
	{
		var user = await _userClient.FindByEmailAsync(userName);

		if (user == null)
		{
			return new ResponseModel<UserLoginResponse>
			{
				IsSuccess = false,
				Message = "Sai mật khẩu"
			};
		}

		var result = await _signInManagerCustomer.PasswordSignInAsync(user, password, true, false);

		if (result.IsNotAllowed)
		{
			return new ResponseModel<UserLoginResponse>
			{
				IsSuccess = false,
				Message = "Không được truy cập"
			};
		}

		if (result.Succeeded)
		{
			string secrectKeyAdmin = _configuration.GetValue<string>("SecrectKeyAdmin")!;
			var token = IdentityUtility.GenerateToken(user, null, secrectKeyAdmin, user.Id.ToString(), user.Name);
			return new ResponseModel<UserLoginResponse>
			{
				IsSuccess = true,
				Data = new UserLoginResponse { Token = token, UserName = user.Name }
			};
		}

		return new ResponseModel<UserLoginResponse>
		{
			IsSuccess = false,
			Message = "Không được truy cập"
		};
	}

	public async Task<ResponseModel<UserLoginResponse>> LoginForAdmin(string userName, string password)
	{
		var bnoi = _user.GetWebHostEnvironment();
		var user = await _userManager.FindByEmailAsync(userName);

		if (user == null)
		{
			return new ResponseModel<UserLoginResponse>
			{
				IsSuccess = false,
				Message = "Sai Mạt khaih"
			};
		}

		var result = await _signInManager.PasswordSignInAsync(user, password, true, false);
		var roles = await _userManager.GetRolesAsync(user);

		if (result.IsLockedOut)
		{
			return new ResponseModel<UserLoginResponse>
			{
				IsSuccess = false,
				Message = "Bị khóa"
			};
		}

		if (result.IsNotAllowed)
		{
			return new ResponseModel<UserLoginResponse>
			{
				IsSuccess = false,
				Message = "Không được truy cập"
			};
		}

		if (result.Succeeded)
		{
			string secrectKeyAdmin = _configuration.GetValue<string>("SecrectKeyAdmin")!;
			var token = IdentityUtility.GenerateToken(user, roles, secrectKeyAdmin, user.Id.ToString(), user.Name);
			return new ResponseModel<UserLoginResponse>
			{
				IsSuccess = true,
				Data = new UserLoginResponse { Token = token, UserName = user.Name }
			};
		}

		return new ResponseModel<UserLoginResponse>
		{
			IsSuccess = false,
			Message = "Không được truy cập"
		};
	}
	#endregion

	#region Infomation account login
	/// <summary>
	/// Get infomation user by admin
	/// </summary>
	/// <param name="userId"></param>
	/// <returns>Infomation use</returns>
	public async Task<ResponseModel<UserDto>> GetInformationUserByAdmin()
	{
		if (string.IsNullOrEmpty(_user.Id) == false)
		{
			var result = await _userManager.FindByIdAsync(_user.Id);
			return new ResponseModel<UserDto>
			{
				IsSuccess = true,
				Data = _mapper.Map<UserDto>(result)
			};
		}
		else
		{
			return new ResponseModel<UserDto>
			{
				IsSuccess = false,
				Data = null
			};
		}
	}

	/// <summary>
	/// Get infomation user by user
	/// </summary>
	/// <returns>Infomation user</returns>
	public async Task<ResponseModel<CustomUserClientDto>> GetInfotmationUserByUser()
	{
		if (string.IsNullOrEmpty(_user.Id) == false)
		{
			var result = await _userClient.FindByIdAsync(_user.Id);
			return new ResponseModel<CustomUserClientDto>
			{
				IsSuccess = true,
				Data = _mapper.Map<CustomUserClientDto>(result)
			};
		}
		else
		{
			return new ResponseModel<CustomUserClientDto>
			{
				IsSuccess = false,
				Data = null
			};
		}
	}
	#endregion

	#region Create User
	/// <summary>
	/// Create user for client
	/// </summary>
	/// <param name="userDto"></param>
	/// <returns></returns>
	public async Task<ResponseModel<CustomUserClientDto>> CreateUserForClientAsync(CustomUserClientDto userDto)
	{
		//var checkExistAccount = await IsExistAccount(userDto.MailAddr, ConstantCommon.CONST_TYPE_ACCOUNT_ADMIN);
		//if (checkExistAccount == false)
		//{
		//    var userClient = _mapper.Map<Rb_CustomerUser>(userDto);

		//    var result = await _userClient.CreateAsync(userClient, userDto.Password!);
		//    if (result.Succeeded)
		//    {

		//        return new ResponseModel<CustomUserClientDto>
		//        {
		//            IsSuccess = true,
		//            Data = _mapper.Map<CustomUserClientDto>(result)
		//        };
		//    }
		//}
		return new ResponseModel<CustomUserClientDto>
		{
			IsSuccess = false,
			Data = null,
			Message = "Tài khoản đã tồn tại"
		};
	}

	/// <summary>
	/// Create user for admin
	/// </summary>
	/// <param name="userDto"></param>
	/// <returns></returns>
	public async Task<ResponseModel<ApplicationUserDto>> CreateUserForAdminAsync(UserCreateDto userDto)
	{
		var checkExistAccount = await IsExistAccount(userDto.MailAddr);
		if (checkExistAccount == false)
		{
			var userApplication = _mapper.Map<ApplicationUser>(userDto);

			var result = await _userManager.CreateAsync(userApplication, userDto.Password!);

			if (result.Succeeded)
			{

				return new ResponseModel<ApplicationUserDto>
				{
					IsSuccess = true,
					Data = _mapper.Map<ApplicationUserDto>(result)
				};
			}
			return new ResponseModel<ApplicationUserDto>
			{
				IsSuccess = false,
				Data = null,
				Message = string.Join("; ", result.Errors.Select(e => e.Description))
			};
		}
		return new ResponseModel<ApplicationUserDto>
		{
			IsSuccess = false,
			Data = null,
			Message = "Tài khoản đã tồn tại"
		};
	}
	#endregion

	#region Update User
	/// <summary>
	/// Update user for admin
	/// </summary>
	/// <param name="userDto"></param>
	/// <returns></returns>
	public async Task<ResponseModel<UserUpdateDto>> UpdateUserForAdminAsync(UserUpdateDto userDto)
	{
		var userClient = _mapper.Map<ApplicationUser>(userDto);

		var result = await _userManager.UpdateAsync(userClient);


		if (result.Succeeded)
		{

			return new ResponseModel<UserUpdateDto>
			{
				IsSuccess = true,
				Data = _mapper.Map<UserUpdateDto>(result)
			};
		}

		return new ResponseModel<UserUpdateDto>
		{
			IsSuccess = false,
			Data = null,
			Message = string.Join("; ", result.Errors.Select(e => e.Description))
		};
	}

	/// <summary>
	/// update user for client
	/// </summary>
	/// <param name="userDto"></param>
	/// <returns></returns>
	public async Task<ResponseModel<UserUpdateDto>> UpdateUserForClientAsync(UserUpdateDto userDto)
	{
		var userClient = _mapper.Map<Rb_CustomerUser>(userDto);

		var result = await _userClient.UpdateAsync(userClient);

		return new ResponseModel<UserUpdateDto>
		{
			IsSuccess = true,
			Data = _mapper.Map<UserUpdateDto>(result)
		};
	}
	#endregion

	#region Delete User
	/// <summary>
	/// Delete user for client
	/// </summary>
	/// <returns></returns>
	public async Task<Result> DeleteUserForClientAsync()
	{
		var userClient = await _userClient.FindByIdAsync(_user.Id!);

		if (userClient != null)
		{
			var result = await _userClient.DeleteAsync(userClient);

			if (result.Succeeded)
			{
				return Result.Success();
			}
		}

		return Result.Failure();
	}

	/// <summary>
	/// Delete user for admin
	/// </summary>
	/// <returns></returns>
	public async Task<Result> DeleteUserForAdminAsync()
	{
		var userAdmin = await _userManager.FindByIdAsync(_user.Id!);

		if (userAdmin != null)
		{
			var result = await _userManager.DeleteAsync(userAdmin);

			if (result.Succeeded)
			{
				return Result.Success();
			}
		}

		return Result.Failure();
	}
	#endregion

	#region Service account role admin
	public async Task<string?> GetUserNameForAdminAsync()
	{
		var user = await _userManager.FindByIdAsync(_user.Id);

		return user?.UserName;
	}

	public async Task<bool> IsInRoleAsync(string userId, string role)
	{
		var user = await _userManager.FindByIdAsync(userId);

		return user != null && await _userManager.IsInRoleAsync(user, role);
	}

	public async Task<bool> AuthorizeAsync(string userId, string policyName)
	{
		var user = await _userManager.FindByIdAsync(userId);

		if (user == null)
		{
			return false;
		}

		var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

		var result = await _authorizationService.AuthorizeAsync(principal, policyName);

		return result.Succeeded;
	}
	#endregion

	#region Check Exists Account
	public async Task<bool> IsExistAccount(string email, string typeAccount = null)
	{
		var checkTypeAccount = IdentityUtility.HandleCheckTypesAccountRequest(_user.IsRoleAdmin(), typeAccount);

		switch (checkTypeAccount)
		{
			default:
				return false;
		}
	}
	#endregion
}

