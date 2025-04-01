using System.ComponentModel.DataAnnotations;

namespace Identity.API.Model.Dtos
{
	public class LoginRequest
	{
		[Required]
		public string UserName { get; set; } = null!;

		[Required]
		public string Password { get; set; } = null!;

	}
}
