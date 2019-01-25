using AdvancedChat.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using AdvancedChat.Dto;
using Microsoft.Extensions.Logging;
using System;

namespace AdvancedChat.Hubs
{
    [Authorize]
    public class ChatHub : Hub<IChatHubClient>
    {
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(ILogger<ChatHub> logger)
        {
            _logger = logger;
        }

        public async Task SendMessage(MessageDto message)
        {
            await Clients.All.ReceiveMessage(message);
        }

        public override Task OnConnectedAsync()
        {
            _logger.LogDebug($"New user connected: {Context.UserIdentifier}");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogDebug($"User: {Context.UserIdentifier} disconnected!");
            return base.OnDisconnectedAsync(exception);
        }
    }
}