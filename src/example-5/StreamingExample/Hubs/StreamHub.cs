using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace StreamingExample.Hubs
{
    public class StreamHub : Hub
    {
        private readonly ILogger<StreamHub> _logger;

        public StreamHub(ILogger<StreamHub> logger)
        {
            _logger = logger;
        }

        //Client to server streaming
        public async Task StartStream(string streamName, ChannelReader<string> streamContent)
        {
            // read from and process stream items
            while (await streamContent.WaitToReadAsync(Context.ConnectionAborted))
            {
                while (streamContent.TryRead(out var content))
                {
                    _logger.LogInformation($"Received {content}!");
                }
            }
        }

        //Server to client streaming
        public ChannelReader<int> Counter(int count, int delay, CancellationToken cancellationToken)
        {
            var channel = Channel.CreateUnbounded<int>();

            // We don't want to await WriteItemsAsync, otherwise we'd end up waiting 
            // for all the items to be written before returning the channel back to
            // the client.
            _ = WriteItemsAsync(channel.Writer, count, delay, cancellationToken);

            return channel.Reader;
        }

        private async Task WriteItemsAsync(ChannelWriter<int> writer, int count, int delay,
            CancellationToken cancellationToken)
        {
            try
            {
                for (var i = 0; i < count; i++)
                {
                    // Check the cancellation token regularly so that the server will stop
                    // producing items if the client disconnects.
                    cancellationToken.ThrowIfCancellationRequested();
                    await writer.WriteAsync(i);

                    // Use the cancellationToken in other APIs that accept cancellation
                    // tokens so the cancellation can flow down to them.
                    await Task.Delay(delay, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                writer.TryComplete(ex);
            }

            writer.TryComplete();
        }
    }
}
