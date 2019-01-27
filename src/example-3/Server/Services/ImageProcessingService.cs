using Microsoft.AspNetCore.SignalR;
using Server.Hubs;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Server.Services
{
    public class ImageProcessingService : IHostedService
    {
        private readonly ILogger<ImageProcessingService> _logger;
        private readonly IHubContext<ImageProcessingHub, IImageProcessingHubClient> _hubContext;

        public ImageProcessingService(ILogger<ImageProcessingService> logger, IHubContext<ImageProcessingHub, IImageProcessingHubClient> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        private async Task ProcessImage()
        {
            await Task.Delay(TimeSpan.FromSeconds(10));
            _logger.LogInformation("Sending processed image to clients!");
            var startTime = DateTime.Now; 
            for (int i = 0; i < 10000; i++)
            {
                var processedImage = new ProcessedImageDto
                {
                    Id = Guid.NewGuid(),
                    Author = "Picasso",
                    Name = $"Test image No. {new Random().Next(1, 666)}"
                };
                await _hubContext.Clients.All.ReceiveProcessedImage(processedImage);
            }
            _logger.LogInformation($"Finished image processing! Time: {DateTime.Now - startTime}");
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start Background Service!");
            await ProcessImage();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Background Service is stopping.");


            return Task.CompletedTask;
        }
    }
}
