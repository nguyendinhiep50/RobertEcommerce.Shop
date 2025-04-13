namespace Shared.Core.Authentication;

public class OpenIdConnectExternalLoginProviderSettings
{
	public string ClientId { get; set; } = string.Empty;

	public string ClientSecret { get; set; } = string.Empty;

	public string Authority { get; set; } = string.Empty;

	public string LoginUrl { get; set; } = string.Empty;

	public bool ValidateIssuer { get; set; }

	public string ResponseType { get; set; } = string.Empty;

	public bool IsValid()
	{
		return !string.IsNullOrEmpty(ClientId)
			|| !string.IsNullOrEmpty(Authority);
	}
}