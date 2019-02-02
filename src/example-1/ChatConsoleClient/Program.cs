using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace ChatConsoleClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello Bug.Net!");
            Console.WriteLine("Let's connect to our chat!");

            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/chatHub")
                .Build();

            connection.On<string, string>("ReceiveMessage", 
                (user, message) => Console.WriteLine($"User: {user} send: {message}"));

            await connection.StartAsync()
                .ContinueWith(_ => Console.WriteLine("Connected!"));

            while(true)
            {
                await Task.Delay(TimeSpan.FromSeconds(10));
                await connection.InvokeAsync("SendMessage", "ConsoleClient", "Greetings to all participants of Bydgoszcz .Net User Group!");
            }
        }
    }
}
