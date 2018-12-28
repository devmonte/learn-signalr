using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Hubs
{
    public interface IImageProcessingHubClient
    {
        Task ReceiveProcessedImage(ProcessedImageDto processedImage);
    }
}
