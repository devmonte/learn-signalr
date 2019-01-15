﻿using Microsoft.AspNetCore.SignalR.Client;
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
                .WithUrl("http://localhost:5000/chatHub", options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(token);
                })
                .ConfigureLogging(logging => {
                    logging.SetMinimumLevel(LogLevel.Debug);
                    logging.AddConsole();
                })
                .Build();

            connection.On<string, string>("ReceiveMessage", (user, message) => Console.WriteLine($"User: {user} send: {message}"));

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
                await connection.InvokeAsync("SendMessage", "ConsoleClient", "Greetings to all participants of Bydgoszcz .Net User Group!");
            }

            Console.WriteLine("Press key to close!");
            Console.ReadKey();
        }

        static async Task<string> GetToken()
        {
            var token = "";
            using (var apiClient = new HttpClient())
            {
                var serializedUser = JsonConvert.SerializeObject(new { Name = "ConsoleClient", Password = "BdgDotNet", Group = "TestGroup" });
                var response = await apiClient.PostAsync("https://localhost:5001/api/auth", new StringContent(serializedUser, Encoding.UTF8, "application/json"));
                token = await response.Content.ReadAsStringAsync();
            }
            Console.WriteLine($"Returning token {token}");
            return token;
        }
    }
}