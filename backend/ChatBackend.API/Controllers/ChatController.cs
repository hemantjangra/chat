using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ChatBackend.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly ILogger<ChatController> _logger;

        private readonly ChatMessageHandler _chatMessageHandler;

        public ChatController(ILogger<ChatController> logger, ChatMessageHandler chatMessageHandler)
        {
            _logger = logger;
            _chatMessageHandler = chatMessageHandler;
        }
        
        [HttpGet("/api/v1/sendMessage")]
        public async Task SendMessage([FromQuery] string message)
        {
            await _chatMessageHandler.SendMessageToAllAsync(message);
        }
    }
}
