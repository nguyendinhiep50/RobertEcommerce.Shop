using Identity.API.Interface;
using Microsoft.Extensions.Options;

namespace Identity.API.Identity.OverrideIdentity;

public class CustomUserClient : UserManager<Rb_CustomerUser>
{
    private readonly IUser _user;

    public CustomUserClient(
        IUserStore<Rb_CustomerUser> store,
        IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<Rb_CustomerUser> passwordHasher,
        IEnumerable<IUserValidator<Rb_CustomerUser>> userValidators,
        IEnumerable<IPasswordValidator<Rb_CustomerUser>> passwordValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        IServiceProvider services,
        ILogger<UserManager<Rb_CustomerUser>> logger,
        IUser user)
        : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {
        _user = user;
    }

    public override async Task<IdentityResult> UpdateAsync(Rb_CustomerUser user)
    {
        user.UpdatedBy = _user.Id;

        return await base.UpdateAsync(user);
    }

    public override async Task<IdentityResult> DeleteAsync(Rb_CustomerUser user)
    {
        return await base.UpdateAsync(user);
    }
}
