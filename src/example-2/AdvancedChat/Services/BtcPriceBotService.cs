using AdvancedChat.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AdvancedChat.Dto;

namespace AdvancedChat.Services
{
    public class BtcPriceBotService : IHostedService
    {
        private readonly IHubContext<ChatHub, IChatHubClient> _hubContext;

        public BtcPriceBotService(IHubContext<ChatHub, IChatHubClient> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            do
            {
                await Task.Delay(TimeSpan.FromMinutes(3), cancellationToken);
                await SendCurrentBtcPrice(cancellationToken);
            } while (!cancellationToken.IsCancellationRequested);
        }

        public async Task SendCurrentBtcPrice(CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                var message = new MessageDto
                {
                    User = "BtcPriceBot",
                    Message = $"Current BTC price: {new Random().Next(3000, 5000)} $!"
                };
                await _hubContext.Clients.All
                    .ReceiveMessage(message);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
