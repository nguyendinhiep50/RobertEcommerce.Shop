using System.Globalization;

namespace RobertEcommerce.Shop.ServiceDefaults.Utility;
public static class IdFormatter
{
	public static (string KeyId, DateTime Date, int Id) ParseFormattedId(string formattedId)
	{
		var parts = formattedId.Split('_');
		if (parts.Length != 2 || parts[1].Length < 8)
			throw new ArgumentException("Định dạng ID không hợp lệ.");

		string keyId = parts[0];
		string datePart = parts[1].Substring(0, 8);
		string idPart = parts[1].Substring(8);

		DateTime date = DateTime.ParseExact(datePart, "yyyyMMdd", CultureInfo.InvariantCulture);
		int id = int.Parse(idPart);

		return (keyId, date, id);
	}

	public static string FormatId(string keyId, int id)
	{
		return $"{keyId}_{DateTime.Now:yyyyMMdd}{id:D6}";
	}

	public static string GenerateSequentialIdByDate(
		string lastFormattedId,
		string keyId)
	{
		var (_, date, id) = ParseFormattedId(lastFormattedId);

		if (date.Date == DateTime.Now.Date)
		{
			return $"{keyId}_{DateTime.Now:yyyyMMdd}{id + 1:D6}";
		}

		return $"{keyId}_{DateTime.Now:yyyyMMdd}{1:D6}";
	}
}
