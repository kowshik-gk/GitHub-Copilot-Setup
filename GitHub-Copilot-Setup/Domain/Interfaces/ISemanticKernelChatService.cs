using OpenAI_DotNet_Integration.Domain.Dtos;

namespace OpenAI_DotNet_Integration.Domain.Interfaces;

public interface ISemanticKernelChatService
{
    Task<string> GetChatResponseAsync(string userMessage, CancellationToken cancellationToken);
}
