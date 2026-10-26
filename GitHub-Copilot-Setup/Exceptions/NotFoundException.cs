namespace GitHub_Copilot_Setup.Exceptions;

public sealed class NotFoundException(string message) : Exception(message)
{
}
