namespace Shared.Core.Authentication;

public class TwitterExternalLoginProviderSettings
{
	public string ConsumerKey { get; set; } = string.Empty;
	public string ConsumerSecret { get; set; } = string.Empty;

	public bool IsValid()
	{
		return !string.IsNullOrEmpty(ConsumerKey) && !string.IsNullOrEmpty(ConsumerSecret);
	}
}