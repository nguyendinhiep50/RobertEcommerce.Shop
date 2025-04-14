namespace Identity.API.Models;

public static class JwtSettings
{
	public static string SecretKey { get; set; } = string.Empty;
	public static string Issuer { get; set; } = string.Empty;
	public static string Audience { get; set; } = string.Empty;
	public static int AccessTokenExpirationMinutes { get; set; }
	public static int RefreshTokenExpirationDays { get; set; }
	public static int ExpiresInMinutes { get; set; }

	public static void Initialize(IConfiguration configuration)
	{
		Issuer = configuration["Issuer"]!;
		Audience = configuration["Audience"]!;
		SecretKey = configuration["SecretKey"]!;
		AccessTokenExpirationMinutes = int.TryParse(configuration["AccessTokenExpirationMinutes"], out var accessToken) ? accessToken : 15;
		RefreshTokenExpirationDays = int.TryParse(configuration["RefreshTokenExpirationDays"], out var refreshToken) ? refreshToken : 7;
		ExpiresInMinutes = int.TryParse(configuration["ExpiresInMinutes"], out var result) ? result : 60;
	}
}