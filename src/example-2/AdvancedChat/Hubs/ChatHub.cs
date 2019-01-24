using AdvancedChat.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using AdvancedChat.Dto;

namespace AdvancedChat.Hubs
{
    [Authorize]
    public class ChatHub : Hub<IChatHubClient>
    {
        public async Task SendMessage(MessageDto message)
        {
            await Clients.All.ReceiveMessage(message);
        }
    }
}