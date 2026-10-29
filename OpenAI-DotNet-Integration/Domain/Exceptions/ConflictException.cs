namespace OpenAI_DotNet_Integration.Domain.Exceptions;

public sealed class ConflictException(string message) : Exception(message)
{
}
