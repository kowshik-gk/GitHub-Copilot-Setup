namespace OpenAI_DotNet_Integration.Domain.Exceptions;

public sealed class NotFoundException(string message) : Exception(message)
{
}
