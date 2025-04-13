namespace Shared.Core.Authentication
{
	public class GoogleExternalLoginProviderSettings
	{
		public string ClientId { get; set; } = string.Empty;
		public string ClientSecret { get; set; } = string.Empty;
		public string UserInfoEndpoint { get; set; } = string.Empty;

		public bool IsValid()
		{
			return !string.IsNullOrEmpty(ClientSecret) && !string.IsNullOrEmpty(ClientId);
		}
	}
}
