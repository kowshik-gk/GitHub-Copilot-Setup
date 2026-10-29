using Microsoft.AspNetCore.Mvc;
using OpenAI_DotNet_Integration.Domain.Interfaces;

namespace OpenAI_DotNet_Integration.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly ISemanticKernelChatService _chatService;
        private readonly ILogger<ChatController> _logger;

        public ChatController(ISemanticKernelChatService chatService, ILogger<ChatController> logger)
        {
            _chatService = chatService;
            _logger = logger;
        }

        /// <summary>
        /// Send a message to the AI chat service and get a response
        /// </summary>
        /// <param name="request">The chat message request</param>  
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The AI response</returns>
        [HttpPost("message")]
        public async Task<ActionResult<ChatMessageResponse>> SendMessage([FromBody] ChatMessageRequest request, CancellationToken cancellationToken)
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
