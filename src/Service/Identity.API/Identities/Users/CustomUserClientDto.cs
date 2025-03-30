namespace Identity.API.Identities.Users
{
    public class CustomUserClientDto : UserUpdateDto
    {
        public string Id { get; set; }
        public string? Password { get; set; }
    }
}
