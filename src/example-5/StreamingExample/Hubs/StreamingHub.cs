using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace StreamingExample.Hubs
{
    public class StreamingHub : Hub
    {
        public async Task StartStream(string streamName, ChannelReader<string> streamContent)
        {
            // read from and process stream items
            while (await streamContent.WaitToReadAsync(Context.ConnectionAborted))
            {
                while (streamContent.TryRead(out var content))
                {
                    // process content
                }
            }
        }

    }
}
