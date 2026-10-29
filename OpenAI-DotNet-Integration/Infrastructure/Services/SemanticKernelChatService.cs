using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OpenAI_DotNet_Integration.Domain.Interfaces;

namespace OpenAI_DotNet_Integration.Infrastructure.Services;

public class SemanticKernelChatService : ISemanticKernelChatService
{
    private readonly Kernel _kernel;
    private readonly IChatCompletionService _chatCompletionService;
    private readonly ChatHistory _chatHistory;
    private readonly OpenAIPromptExecutionSettings _executionSettings;
    private readonly ILogger<SemanticKernelChatService> _logger;

    public SemanticKernelChatService(IConfiguration configuration, ILogger<SemanticKernelChatService> logger)
    {
        _logger = logger;

        var modelId = configuration["AzureOpenAI:ModelId"];
        var endpoint = configuration["AzureOpenAI:Endpoint"];
        var apiKey = configuration["AzureOpenAI:ApiKey"];

        if (string.IsNullOrWhiteSpace(modelId))
        {
            throw new InvalidOperationException("AzureOpenAI:ModelId not configured");
        }

        if (string.IsNullOrWhiteSpace(endpoint))
        {
            throw new InvalidOperationException("AzureOpenAI:Endpoint not configured");
        }

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("AzureOpenAI:ApiKey not configured. Set AzureOpenAI__ApiKey as an environment variable or use user secrets.");
        }

        try
        {
            // NVIDIA exposes an OpenAI-compatible API, not an Azure OpenAI deployment endpoint.
            var builder = Kernel.CreateBuilder()
                .AddOpenAIChatCompletion(
                    modelId,
                    new Uri(endpoint),
                    apiKey,
                    orgId: null,
                    serviceId: null,
                    httpClient: null);

            // Add enterprise components
            builder.Services.AddLogging(services => services.AddConsole().SetMinimumLevel(LogLevel.Trace));

            // Build the kernel
            _kernel = builder.Build();
            _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();

            // Add a plugin (the LightsPlugin class - uncomment if you have it)
            // _kernel.Plugins.AddFromType<LightsPlugin>("Lights");

            // Enable planning
            _executionSettings = new OpenAIPromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            };

            // Create a history store for the conversation
            _chatHistory = new ChatHistory();

            _logger.LogInformation("SemanticKernelChatService initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing SemanticKernelChatService");
            throw;
        }
    }

    public async Task<string> GetChatResponseAsync(string userMessage, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userMessage))
        {
            throw new ArgumentException("User message cannot be empty", nameof(userMessage));
        }

        try
        {
            // Add user input to chat history
            _chatHistory.AddUserMessage(userMessage);

            _logger.LogInformation("Processing user message: {UserMessage}", userMessage);

            // Get the response from the AI
            var result = await _chatCompletionService.GetChatMessageContentAsync(
                _chatHistory,
                executionSettings: _executionSettings,
                kernel: _kernel,
                cancellationToken: cancellationToken);

            // Add the message from the agent to the chat history
            _chatHistory.AddMessage(result.Role, result.Content ?? string.Empty);

            var response = result.Content ?? string.Empty;
            _logger.LogInformation("Generated response: {Response}", response);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting chat response");
            throw;
        }
    }
}
