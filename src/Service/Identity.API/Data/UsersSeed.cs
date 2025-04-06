namespace Identity.API.Data;

public class UsersSeed(ILogger<UsersSeed> logger, UserManager<ApplicationUser> userManager) : IDbSeeder<ApplicationDbContext>
{
	public async Task SeedAsync(ApplicationDbContext context)
	{
		var alice = await userManager.FindByNameAsync("Robert");
		if (alice == null)
		{
			alice = new ApplicationUser
			{
				Id = IdFormatter.FormatId(Constant.CONSTANT_KEY_USER_ID, 1),
				UserName = "Robert",
				Email = "Robert@email.com",
				EmailConfirmed = true,
				Name = "Robert Nguyen",
				CreatedDate = DateTime.UtcNow,
				CreatedBy = "Admin"
			};

			var result = userManager.CreateAsync(alice, "Pass123$").Result;

			if (!result.Succeeded)
			{
				throw new Exception(result.Errors.First().Description);
			}

			if (logger.IsEnabled(LogLevel.Debug))
			{
				logger.LogDebug("Robert created");
			}
		}
		else
		{
			if (logger.IsEnabled(LogLevel.Debug))
			{
				logger.LogDebug("Robert already exists");
			}
		}
	}
}
