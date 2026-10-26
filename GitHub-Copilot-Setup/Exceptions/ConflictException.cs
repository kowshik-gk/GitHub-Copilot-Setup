namespace GitHub_Copilot_Setup.Exceptions;

public sealed class ConflictException(string message) : Exception(message)
{
}
