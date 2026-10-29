using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using OllamaSharp;
using OpenAI_DotNet_Integration.Domain.Interfaces;

namespace OpenAI_DotNet_Integration.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly ISemanticKernelChatService _chatService;
        private readonly ILogger<ChatController> _logger;

        private readonly IConfiguration _configuration;

        public ChatController(ISemanticKernelChatService chatService, ILogger<ChatController> logger, IConfiguration configuration)
        {
            _chatService = chatService;
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Send a message to the AI chat service and get a response
        /// </summary>
        /// <param name="request">The chat message request</param>  
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The AI response</returns>
        [HttpPost("NvidiaModelToChat")]
        public async Task<ActionResult<ChatMessageResponse>> NvidiaModelToChat([FromBody] ChatMessageRequest request, CancellationToken cancellationToken)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest("Message cannot be empty");
            }

            try
            {
                var response = await _chatService.GetChatResponseAsync(request.Message, cancellationToken);
                return Ok(new ChatMessageResponse { Message = response });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing chat message");
                return StatusCode(500, "An error occurred while processing your message");
            }
        }

        [HttpPost("PhiLocalModelToChat")]
        public async Task<ActionResult<ChatResponse>> PhiLocalModelToChat([FromBody] ChatMessageRequest request, CancellationToken cancellationToken)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest("Message cannot be empty");
            }

            try
            {
                string ollamaEndpoint = _configuration["Olamma:Endpoint"] ?? "http://localhost:11434"; // Or use a local model with Ollama
                IChatClient client = new OllamaApiClient(new Uri(ollamaEndpoint), "phi4-mini"); //The default local URL for a local Ollama server

                var reposne =  await client.GetResponseAsync(request.Message);
                return reposne;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing chat message");
                return StatusCode(500, "An error occurred while processing your message");
            }
        }
    }

    public class ChatMessageRequest
    {
        public required string Message { get; set; }
    }

    public class ChatMessageResponse
    {
        public required string Message { get; set; }
    }
}
