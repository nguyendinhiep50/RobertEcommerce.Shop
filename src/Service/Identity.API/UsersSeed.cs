namespace Identity.API;

public class UsersSeed(ILogger<UsersSeed> logger, UserManager<ApplicationUser> userManager) : IDbSeeder<ApplicationDbContext>
{
	public async Task SeedAsync(ApplicationDbContext context)
	{
		var alice = await userManager.FindByNameAsync("alice");
		var dateTimeNowString = Constant.CONSTANT_KEY_USER_ID + DateTime.Now.ToString("yyyyMMdd");
		dateTimeNowString = dateTimeNowString + "00001";
		if (alice == null)
		{
			alice = new ApplicationUser
			{
				Id = dateTimeNowString,
				UserName = "alice",
				Email = "AliceSmith@email.com",
				EmailConfirmed = true,
				Name = "123"
			};

			var result = userManager.CreateAsync(alice, "Pass123$").Result;

			if (!result.Succeeded)
			{
				throw new Exception(result.Errors.First().Description);
			}

			if (logger.IsEnabled(LogLevel.Debug))
			{
				logger.LogDebug("alice created");
			}
		}
		else
		{
			if (logger.IsEnabled(LogLevel.Debug))
			{
				logger.LogDebug("alice already exists");
			}
		}

		var bob = await userManager.FindByNameAsync("bob");

		if (bob == null)
		{
			bob = new ApplicationUser
			{
				Id = dateTimeNowString,
				UserName = "bob",
				Email = "BobSmith@email.com",
				EmailConfirmed = true,
				Name = "123"
			};

			var result = await userManager.CreateAsync(bob, "Pass123$");

			if (!result.Succeeded)
			{
				throw new Exception(result.Errors.First().Description);
			}

			if (logger.IsEnabled(LogLevel.Debug))
			{
				logger.LogDebug("bob created");
			}
		}
		else
		{
			if (logger.IsEnabled(LogLevel.Debug))
			{
				logger.LogDebug("bob already exists");
			}
		}
	}
}
