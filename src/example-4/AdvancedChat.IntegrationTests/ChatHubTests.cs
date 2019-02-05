using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Web.CodeGeneration;
using Xunit;

namespace AdvancedChat.IntegrationTests
{
    public class ChatHubTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _webAppFactory;
        private readonly HttpClient _appClient;
        private readonly Uri _baseAddress;

        public ChatHubTests(WebApplicationFactory<Startup> factory)
        {
            _webAppFactory = factory;
            _appClient = _webAppFactory.CreateClient();
            _baseAddress = _webAppFactory.Server.BaseAddress;
        }

        [Fact]
        public async Task GetHealthCheck_ShouldReturnSuccess()
        {
            //Arrange

            //Act
            var response = await _appClient.GetAsync("/health");

            //Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal("Healthy", content);
        }

        [Fact]
        public async Task ConnectToHubWithoutToken_ShouldThrowException()
        {
            //Arrange
            var hubConnection = new HubConnectionBuilder()
                .WithUrl($"{_baseAddress}chatHub", opt =>
                {
                    opt.HttpMessageHandlerFactory = _ => _webAppFactory.Server.CreateHandler();
                    opt.Transports = HttpTransportType.LongPolling | HttpTransportType.ServerSentEvents;
                })
                .Build();

            Exception expectedException = null;

            //Act
            await hubConnection.StartAsync().ContinueWith(result =>
            {
                expectedException = result.Exception;
            });

            //Assert
            Assert.Equal("Response status code does not indicate success: 401 (Unauthorized).", expectedException.InnerException.Message);
        }

        [Fact]
        public async Task ConnectToHubWithToken_ShouldSuccessfullyConnect()
        {
            //Arrange
            var serializedUser = JsonConvert.SerializeObject(new { Name = "ConsoleClient", Password = "dotnet" });
            var response = await _appClient.PostAsync("api/auth", new StringContent(serializedUser, Encoding.UTF8, "application/json"));
            var token = await response.Content.ReadAsStringAsync();

            var hubConnection = new HubConnectionBuilder()
                .WithUrl($"{_baseAddress}chatHub", opt =>
                {
                    opt.HttpMessageHandlerFactory = _ => _webAppFactory.Server.CreateHandler();
                    opt.Transports = HttpTransportType.LongPolling | HttpTransportType.ServerSentEvents;
                    opt.AccessTokenProvider = () => Task.FromResult(token);
                })
                .Build();

            //Act
            await hubConnection.StartAsync();

            //Assert
            Assert.Equal(HubConnectionState.Connected, hubConnection.State);
        }

    }
}

