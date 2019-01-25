using AdvancedChat;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AdvancedChat.IntegrationTests
{
    public class ChatHubTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _webAppFactory;

        public ChatHubTests(WebApplicationFactory<Startup> factory)
        {
            _webAppFactory = factory;
        }

        [Fact]
        public async Task GetHealthCheck_ShouldReturnSuccess()
        {
            //Arrange
            var client = _webAppFactory.CreateClient();

            //Act
            var response = await client.GetAsync("/health");

            //Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal("Healthy", content);
        }

        [Fact]
        public async Task ConnectToHubWithoutToken_ShouldThrowException()
        {
            //Arrange
            _webAppFactory.CreateClient();
            var baseAddress = _webAppFactory.Server.BaseAddress;
            var hubConnection = new HubConnectionBuilder()
                .WithUrl($"{baseAddress}chatHub", opt =>
                {
                    opt.HttpMessageHandlerFactory = _ => _webAppFactory.Server.CreateHandler();
                    opt.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling | Microsoft.AspNetCore.Http.Connections.HttpTransportType.ServerSentEvents;
                })
                .Build();

            Exception expectedException = null;

            //Act
            await hubConnection.StartAsync().ContinueWith(result =>
            {
                expectedException = result.Exception;
            });

            //Assert
            expectedException.InnerException
                .Message.Should().Be("Response status code does not indicate success: 401 (Unauthorized).");
        }

        [Fact]
        public async Task ConnectToHubWithToken_ShouldSuccessfullyConnect()
        {
            //Arrange
            var client = _webAppFactory.CreateClient();

            var serializedUser = JsonConvert.SerializeObject(new { Name = "ConsoleClient", Password = "BdgDotNet", Group = "TestGroup" });
            var response = await client.PostAsync("api/auth", new StringContent(serializedUser, Encoding.UTF8, "application/json"));
            var token = await response.Content.ReadAsStringAsync();

            var baseAddress = _webAppFactory.Server.BaseAddress;
            var hubConnection = new HubConnectionBuilder()
                .WithUrl($"{baseAddress}chatHub", opt =>
                {
                    opt.HttpMessageHandlerFactory = _ => _webAppFactory.Server.CreateHandler();
                    opt.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling | Microsoft.AspNetCore.Http.Connections.HttpTransportType.ServerSentEvents;
                    opt.Headers.Add("access_token", token);
                })
                .Build();

            //Act
            await hubConnection.StartAsync();

            //Assert
            hubConnection.State.Should().Be(HubConnectionState.Connected);
        }

    }
}
