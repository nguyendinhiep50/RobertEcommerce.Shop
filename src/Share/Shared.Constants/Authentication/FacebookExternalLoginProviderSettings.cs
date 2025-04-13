namespace Shared.Core.Authentication;

public class FacebookExternalLoginProviderSettings
{
	public string AppId { get; set; } = string.Empty;
	public string AppSecret { get; set; } = string.Empty;

	public bool IsValid()
	{
		return !string.IsNullOrEmpty(AppId) && !string.IsNullOrEmpty(AppSecret);
	}
}