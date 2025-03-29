namespace Identity.API.Identities.Users
{
	public class CustomUserClientDto : UserUpdateDto
	{
		public Guid Id { get; set; }
		public string? Password { get; set; }
	}
}
