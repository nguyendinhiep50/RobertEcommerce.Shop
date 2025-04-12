namespace Identity.API.Data;

public class SeedData(
	ILogger<SeedData> logger,
	UserManager<ApplicationUser> userManager,
	RoleManager<ApplicationRole> roleManager
) : IDbSeeder<ApplicationDbContext>
{
	public async Task UserSeedAsync(ApplicationDbContext context)
	{
		await SeedUsersAsync(context);
		await SeedRolesAsync(context);
	}

	private async Task SeedUsersAsync(ApplicationDbContext context)
	{
		var robert = await userManager.FindByNameAsync("Robert");
		if (robert == null)
		{
			robert = new ApplicationUser
			{
				Id = IdFormatter.FormatId(Constant.CONSTANT_KEY_USER_ID, 1),
				UserName = "Robert",
				Email = "Robert@email.com",
				EmailConfirmed = true,
				Name = "Robert Nguyen",
				CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
				CreatedBy = "Admin",
			};
			var result = await userManager.CreateAsync(robert, "Pass123$");

			var strategy = context.Database.CreateExecutionStrategy();
			await strategy.ExecuteAsync(async () =>
			{
				await using var transaction = await context.Database.BeginTransactionAsync();

				try
				{
					var users = new List<ApplicationUser>();

					var superadmins = new ApplicationUser
					{
						Id = IdFormatter.FormatId(Constant.CONSTANT_KEY_USER_ID, 2),
						Name = "Super Admin",
						UserName = "superadmin@gmail.com",
						NormalizedUserName = "SUPERADMIN@GMAIL.COM",
						PhoneNumber = "0936333401",
						Email = "superadmin@gmail.com",
						NormalizedEmail = "SUPERADMIN@GMAIL.COM",
						PhoneNumberConfirmed = true,
						EmailConfirmed = true,
					};
					superadmins.PasswordHash = userManager.PasswordHasher.HashPassword(superadmins, "Pass123$");
					users.Add(superadmins);

					for (int i = 3; i <= 100; i++)
					{
						var user = new ApplicationUser
						{
							Id = IdFormatter.FormatId(Constant.CONSTANT_KEY_USER_ID, i),
							Name = $"User {i}",
							UserName = $"user{i}@example.com",
							NormalizedUserName = $"USER{i}@EXAMPLE.COM",
							PhoneNumber = $"0900000{i:D3}",
							Email = $"user{i}@example.com",
							NormalizedEmail = $"USER{i}@EXAMPLE.COM",
							PhoneNumberConfirmed = true,
							EmailConfirmed = true,
							CreatedBy = Roles.Administrator,
						};

						user.PasswordHash = userManager.PasswordHasher.HashPassword(user, "Pass123$");
						users.Add(user);
					}

					await context.Users.AddRangeAsync(users);

					await context.SaveChangesAsync();
					await transaction.CommitAsync();
				}
				catch (Exception)
				{
					await transaction.RollbackAsync();
					throw;
				}
			});

			if (!result.Succeeded)
				throw new Exception(result.Errors.First().Description);

			logger.LogInformation("User 'Robert' created.");

		}
	}

	private async Task SeedRolesAsync(ApplicationDbContext context)
	{
		var strategy = context.Database.CreateExecutionStrategy();

		await strategy.ExecuteAsync(async () =>
		{
			await using var transaction = await context.Database.BeginTransactionAsync();

			var roleNames = new[] { "Administrator", "Manager", "User" };
			foreach (var roleName in roleNames)
			{
				if (!await roleManager.RoleExistsAsync(roleName))
				{
					var result = await roleManager.CreateAsync(new ApplicationRole
					{
						Id = Guid.NewGuid().ToString(),
						Name = roleName,
						NormalizedName = roleName.ToUpper(),
						ConcurrencyStamp = Guid.NewGuid().ToString()
					});

					if (!result.Succeeded)
						throw new Exception($"Error creating role {roleName}");

					logger.LogInformation($"Role '{roleName}' created.");
				}
			}

			await transaction.CommitAsync();
		});
	}

}
