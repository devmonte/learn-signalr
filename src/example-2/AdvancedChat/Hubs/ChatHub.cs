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

        public override async Task OnConnectedAsync()
        {
            var notificationMessage = $"New user connected: {Context.UserIdentifier}";
            _logger.LogDebug(notificationMessage);
            await Clients.AllExcept(Context.UserIdentifier).ReceiveNotification(notificationMessage);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var notificationMessage = $"User: {Context.UserIdentifier} disconnected!";
            _logger.LogDebug(notificationMessage);
            await Clients.All.ReceiveNotification(notificationMessage);
            await base.OnDisconnectedAsync(exception);
        }
    }
}