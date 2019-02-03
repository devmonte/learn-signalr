using AdvancedChat.Dto;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedChatConsoleClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello Bug.Net!");
            Console.WriteLine("Let's connect to our chat!");


            var token = await GetToken();
            var connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:5001/chatHub", options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(token);
                })
                .ConfigureLogging(logging =>
                {
                    logging.SetMinimumLevel(LogLevel.Debug);
                    logging.AddConsole();
                })
                .Build();

            connection.On<MessageDto>("ReceiveMessage", (message) => Console.WriteLine($"User: {message.User} send: {message.Message}"));

            await connection.StartAsync()
                .ContinueWith((result) =>
                {
                    if (!result.IsCompletedSuccessfully)
                    {
                        Console.WriteLine($"Connection failure!");
                        return;
                    }
                    Console.WriteLine("Connected!");
                });

            while (connection.State.Equals(HubConnectionState.Connected))
            {
                await Task.Delay(TimeSpan.FromSeconds(10));
                var message = new { User = "ConsoleClient", Message = "Greetings to all participants of Bydgoszcz .Net User Group!" };
                await connection.InvokeAsync("SendMessage", message);
            }

            Console.WriteLine("Press key to close!");
            Console.ReadKey();
            await connection.StopAsync();
        }

        static async Task<string> GetToken()
        {
            var token = "";
            using (var apiClient = new HttpClient())
            {
                var serializedUser = JsonConvert.SerializeObject(new { Name = "ConsoleClient", Password = "dotnet" } );
                var response = await apiClient.PostAsync("https://localhost:5001/api/auth", new StringContent(serializedUser, Encoding.UTF8, "application/json"));
                token = await response.Content.ReadAsStringAsync();
            }
            Console.WriteLine($"Returning token {token}");
            return token;
        }
    }
}
