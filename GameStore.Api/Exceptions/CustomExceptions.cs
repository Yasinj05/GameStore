namespace GameStore.Api.Exceptions;

public class NotFoundException(string message) : Exception(message);

public class ValidationException(IDictionary<string, string[]> errors)
    : Exception("یک یا چند خطای اعتبارسنجی رخ داده است.")
{
    public IDictionary<string, string[]> Errors { get; } = errors;
}