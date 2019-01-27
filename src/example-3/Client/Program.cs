using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Server.Models;

namespace Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello BUG.Net!");

            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/imagehub")
                //.AddMessagePackProtocol()
                .Build();

            connection.On<ProcessedImageDto>("ReceiveProcessedImage", image =>
            {
                //Task.Run(() =>
                //{
                    Console.WriteLine($"{DateTime.Now} {Thread.CurrentThread.ManagedThreadId} Received image: {image.Name}");
                    Thread.Sleep(10);
               // });
            });
            try
            {
                await connection.StartAsync();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            await Task.Delay(TimeSpan.FromMinutes(1));
            await connection.StopAsync();
            Console.WriteLine("disposing hub connection!");
            await connection.DisposeAsync();

            await Task.Delay(TimeSpan.FromMinutes(1));

            Console.ReadKey();
            Console.WriteLine("Closing app!");
        }
    }
}
