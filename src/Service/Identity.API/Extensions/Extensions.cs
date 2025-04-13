using Identity.API.Identity.OverrideIdentity;
using Identity.API.Models;
using Identity.API.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

public static class Extensions
{
	public static void AddApplicationServices(this IHostApplicationBuilder builder)
	{
		if (builder.Environment.IsBuild())
		{
			builder.Services.AddDbContext<ApplicationDbContext>();
			return;
		}

		builder.AddNpgsqlDbContext<ApplicationDbContext>("identityDb", configureDbContextOptions: dbContextOptionsBuilder =>
		{
			dbContextOptionsBuilder.UseNpgsql(builder =>
			{
				builder.UseVector();
			});
		});
	}

	public static IServiceCollection AddInfrastructureServices(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		services.AddMigration<ApplicationDbContext, SeedData>();

		JwtSettings.Initialize(configuration);

		services.AddAutoMapper(Assembly.GetExecutingAssembly());

		services.AddAuthentication(options =>
		{
			options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
		})
		.AddCookie()
		.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
		{
			options.ClientId = configuration.GetSection("GoogleKeys:ClientId").Value!;
			options.ClientSecret = configuration.GetSection("GoogleKeys:ClientSecret").Value!;
		});

		services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		})
		.AddJwtBearer(options =>
		{
			var key = Encoding.ASCII.GetBytes(JwtSettings.SecretKey);
			options.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = false,
				ValidateAudience = false,
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(key)
			};
		});

		services.AddScoped<CustomUserManager>(provider =>
		{
			var userStore = provider.GetRequiredService<IUserStore<ApplicationUser>>();
			var optionsAccessor = provider.GetRequiredService<IOptions<IdentityOptions>>();
			var passwordHasher = provider.GetRequiredService<IPasswordHasher<ApplicationUser>>();
			var userValidators = provider.GetServices<IUserValidator<ApplicationUser>>();
			var passwordValidators = provider.GetServices<IPasswordValidator<ApplicationUser>>();
			var keyNormalizer = provider.GetRequiredService<ILookupNormalizer>();
			var errors = provider.GetRequiredService<IdentityErrorDescriber>();
			var services = provider;
			var logger = provider.GetRequiredService<ILogger<UserManager<ApplicationUser>>>();
			var user = provider.GetRequiredService<IUser>();
			return new CustomUserManager(userStore, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger, user);
		});

		services.AddScoped<CustomUserClient>(provider =>
		{
			var userStore = provider.GetRequiredService<IUserStore<Rb_CustomerUser>>();
			var optionsAccessor = provider.GetRequiredService<IOptions<IdentityOptions>>();
			var passwordHasher = provider.GetRequiredService<IPasswordHasher<Rb_CustomerUser>>();
			var userValidators = provider.GetServices<IUserValidator<Rb_CustomerUser>>();
			var passwordValidators = provider.GetServices<IPasswordValidator<Rb_CustomerUser>>();
			var keyNormalizer = provider.GetRequiredService<ILookupNormalizer>();
			var errors = provider.GetRequiredService<IdentityErrorDescriber>();
			var services = provider;
			var logger = provider.GetRequiredService<ILogger<UserManager<Rb_CustomerUser>>>();
			var user = provider.GetRequiredService<IUser>();
			return new CustomUserClient(userStore, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger, user);
		});

		services.AddIdentity<ApplicationUser, ApplicationRole>()
			.AddEntityFrameworkStores<ApplicationDbContext>()
			.AddDefaultTokenProviders();

		services.AddScoped<IIdentityService, IdentityService>();
		services.AddScoped<IRoleService, RoleService>();

		services.AddAuthorization();
		services.AddScoped<IUser, CurrentUser>();

		services.AddIdentityCore<ApplicationUser>(options => { })
			.AddRoles<ApplicationRole>()
			.AddEntityFrameworkStores<ApplicationDbContext>()
			.AddSignInManager<SignInManager<ApplicationUser>>()
			.AddDefaultTokenProviders();

		services.AddIdentityCore<Rb_CustomerUser>(options => { })
			.AddEntityFrameworkStores<ApplicationDbContext>()
			.AddSignInManager<SignInManager<Rb_CustomerUser>>()
			.AddDefaultTokenProviders();

		return services;
	}
}
