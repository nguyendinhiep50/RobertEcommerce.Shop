using Identity.API.Interface;
using Microsoft.Extensions.Options;

namespace Identity.API.Identity.OverrideIdentity;

public class CustomUserManager : UserManager<ApplicationUser>
{
	private readonly IUser _user;

	public CustomUserManager(
		IUserStore<ApplicationUser> store,
		IOptions<IdentityOptions> optionsAccessor,
		IPasswordHasher<ApplicationUser> passwordHasher,
		IEnumerable<IUserValidator<ApplicationUser>> userValidators,
		IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
		ILookupNormalizer keyNormalizer,
		IdentityErrorDescriber errors,
		IServiceProvider services,
		ILogger<UserManager<ApplicationUser>> logger,
		IUser user)
		: base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
	{
		_user = user;
	}

	public override async Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
	{
		user.CreatedBy = _user.Id;
		user.UpdatedBy = _user.Id;

		return await base.CreateAsync(user, password);
	}

	public override async Task<IdentityResult> DeleteAsync(ApplicationUser user)
	{
		return await base.UpdateAsync(user);
	}
}
