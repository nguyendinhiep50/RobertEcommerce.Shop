using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Identity.API.Identity
{
	public static class IdentityUtility
	{
		public static string GenerateToken<T>(T user, IList<string> roles, string secretKey, string idUser, string userName)
		{
			if (user == null) throw new ArgumentNullException(nameof(user));

			var handler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(secretKey);
			var credentials = new SigningCredentials(
				new SymmetricSecurityKey(key),
				SecurityAlgorithms.HmacSha256Signature);

			if (roles == null && typeof(T) == typeof(Rb_CustomerUser))
			{
				roles = new List<string>();
				roles.Add(Roles.Customer);
			}

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = GenerateClaims(user, roles, idUser, userName),
				Expires = DateTime.UtcNow.AddDays(15),
				SigningCredentials = credentials,
			};

			var token = handler.CreateToken(tokenDescriptor);
			return handler.WriteToken(token);
		}

		private static ClaimsIdentity GenerateClaims<T>(T user, IList<string> roles, string idUser, string userName)
		{
			var claims = new ClaimsIdentity();

			claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, idUser ?? string.Empty));
			claims.AddClaim(new Claim(ClaimTypes.Name, userName ?? string.Empty));

			foreach (var role in roles)
			{
				if (!string.IsNullOrWhiteSpace(role))
				{
					claims.AddClaim(new Claim(ClaimTypes.Role, role));
				}
			}

			return claims;
		}

		public static string HandleCheckTypesAccountRequest(bool roleUserRequest, string? typeAccount)
		{
			return string.Empty;
		}
	}
}
