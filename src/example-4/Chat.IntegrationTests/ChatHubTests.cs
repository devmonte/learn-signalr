using Chat;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Chat.IntegrationTests
{
    public class ChatHubTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _webAppFactory;

        public ChatHubTests(WebApplicationFactory<Startup> factory)
        {
            _webAppFactory = factory;
        }

        [Fact]
        public async Task ConnectToHub_ShouldConnect()
        {
            //Arrange
            _webAppFactory.CreateClient();
            var baseAddress = _webAppFactory.Server.BaseAddress;
            var hubConnection = new HubConnectionBuilder()
                .WithUrl($"{baseAddress}chatHub", opt =>
                {
                    opt.HttpMessageHandlerFactory = _ => _webAppFactory.Server.CreateHandler();
                    opt.Transports = HttpTransportType.LongPolling | HttpTransportType.ServerSentEvents | HttpTransportType.WebSockets;
                })
                .Build();

            //Act
            await hubConnection.StartAsync();

            //Assert
            Assert.Equal(HubConnectionState.Connected, hubConnection.State);
        }

        [Fact]
        public async Task SendMessageToHub_ShouldPropagateMessageFurther()
        {
            //Arrange
            _webAppFactory.CreateClient();
            var baseAddress = _webAppFactory.Server.BaseAddress;
            var hubConnection = new HubConnectionBuilder()
                .WithUrl($"{baseAddress}chatHub", opt =>
                {
                    opt.HttpMessageHandlerFactory = _ => _webAppFactory.Server.CreateHandler();
                    opt.Transports = HttpTransportType.LongPolling | HttpTransportType.ServerSentEvents | HttpTransportType.WebSockets;
                })
                .Build();

            var messageToSend = "Hello Guys!";
            var receivedMessage = string.Empty;

            hubConnection.On<string, string>("ReceiveMessage", (user, message) => receivedMessage = message);

            //Act
            await hubConnection.StartAsync();
            await hubConnection.InvokeAsync("SendMessage", "TestUser", messageToSend);

            //Assert
            Assert.Equal(messageToSend, receivedMessage);
        }
    }
}
