using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace StreamingconsoleClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:5011/streamingHub")
                .Build();

            // Call "Cancel" on this CancellationTokenSource to send a cancellation message to 
            // the server, which will trigger the corresponding token in the Hub method.
            await hubConnection.StartAsync();

            var cancellationTokenSource = new CancellationTokenSource();
            var channel = await hubConnection.StreamAsChannelAsync<int>(
                "Counter", 10, 500, cancellationTokenSource.Token);

            // Wait asynchronously for data to become available
            while (await channel.WaitToReadAsync())
            {
                // Read all currently available data synchronously, before waiting for more data
                while (channel.TryRead(out var count))
                {
                    Console.WriteLine($"{count}");
                }
            }

            Console.WriteLine("Streaming completed");

            Console.ReadKey();
        }
    }
}
