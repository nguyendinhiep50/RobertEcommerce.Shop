namespace Identity.API.Identities.Dtos;
public class Result
{
	internal Result(bool succeeded, IEnumerable<string> errors)
	{
		Succeeded = succeeded;
		Errors = errors?.ToArray() ?? Array.Empty<string>();
	}

	public bool Succeeded { get; init; }

	public string[] Errors { get; init; }

	public static Result Success()
	{
		return new Result(true, Array.Empty<string>());
	}

	public static Result Failure(IEnumerable<string> errors = null)
	{
		return new Result(false, errors);
	}
}
