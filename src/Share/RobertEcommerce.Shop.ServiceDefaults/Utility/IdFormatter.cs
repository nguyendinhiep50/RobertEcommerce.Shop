namespace RobertEcommerce.Shop.ServiceDefaults.Utility
{
	public static class IdFormatter
	{
		public static string FormatProductId(string keyId, int id)
		{
			return $"{keyId}{DateTime.Now.ToString("yyyyMMdd")}{id:D6}";
		}
	}
}
