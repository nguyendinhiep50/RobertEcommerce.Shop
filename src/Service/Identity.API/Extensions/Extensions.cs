using Microsoft.AspNetCore.Authentication.JwtBearer;
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

	public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddAutoMapper(Assembly.GetExecutingAssembly());

		services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		})
		.AddJwtBearer(options =>
		{
			var key = Encoding.ASCII.GetBytes(configuration.GetValue<string>(Constant.CONSTANT_HASH_KEY_IDENTITY)!);
			options.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = false,
				ValidateAudience = false,
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(key)
			};
		});

		return services;
	}

}
