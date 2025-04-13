namespace Shared.Core.Authentication;

public class MicrosoftExternalLoginProviderSettings
{
	public string ClientId { get; set; } = string.Empty;
	public string ClientSecret { get; set; } = string.Empty;

	public bool IsValid()
	{
		return !string.IsNullOrEmpty(ClientSecret) && !string.IsNullOrEmpty(ClientId);
	}
}
