using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Xunit;

namespace auth_server.Tests
{
    public class EIDModuleTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public EIDModuleTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetChallenge_WithValidClientId_ReturnsChallengeData()
        {
            // Arrange
            string clientId = "test-client";

            // Act
            var response = await _client.GetAsync($"/auth/challenge?clientId={clientId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<ChallengeDataResponse>();

            result.Should().NotBeNull();
            result.challenge.Should().NotBeNull();
            result.salt.Should().NotBeNull();
            result.timestampwindow.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetChallenge_WithoutClientId_ReturnsBadRequest()
        {
            // Act
            var response = await _client.GetAsync("/auth/challenge");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        private class ChallengeDataResponse
        {
            public byte[] challenge { get; set; } = Array.Empty<byte>();
            public byte[] salt { get; set; } = Array.Empty<byte>();
            public string timestampwindow { get; set; } = string.Empty;
        }
    }
}
