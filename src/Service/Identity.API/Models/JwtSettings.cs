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
		var section = configuration.GetSection("JwtSettings");

		Issuer = section["Issuer"]!;
		Audience = section["Audience"]!;
		SecretKey = section["SecretKey"]!;
		AccessTokenExpirationMinutes = int.TryParse(section["AccessTokenExpirationMinutes"], out var accessToken) ? accessToken : 15;
		RefreshTokenExpirationDays = int.TryParse(section["RefreshTokenExpirationDays"], out var refreshToken) ? refreshToken : 7;
		ExpiresInMinutes = int.TryParse(section["ExpiresInMinutes"], out var result) ? result : 60;
	}
}