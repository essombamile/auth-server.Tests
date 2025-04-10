using auth_server.Data;
using auth_server.Models;
using AuthServer.Data;
using MongoDB.Driver;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace auth_server.Tests
{
    public class AuthRepositoryTests
    {
        private readonly Mock<IMongoCollection<ClientAuthData>> _mockCollection;
        private readonly Mock<MongoDbContext> _mockContext;
        private readonly AuthRepository _authRepository;

        public AuthRepositoryTests()
        {
            _mockCollection = new Mock<IMongoCollection<ClientAuthData>>();
            _mockContext = new Mock<MongoDbContext>();
            _mockContext.Setup(c => c.Clients).Returns(_mockCollection.Object);

            _authRepository = new AuthRepository(_mockContext.Object);
        }

        [Fact]
        public async Task GetByClientIdAsync_ShouldReturnClient_WhenClientExists()
        {
            // Arrange
            var clientId = "test-client-id";
            var expectedClient = new ClientAuthData { ClientId = clientId, Email = "test@example.com" };
            _mockCollection.Setup(c => c.Find(It.IsAny<FilterDefinition<ClientAuthData>>())
                .FirstOrDefaultAsync(default)).ReturnsAsync(expectedClient);

            // Act
            var result = await _authRepository.GetByClientIdAsync(clientId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(clientId, result.ClientId);
        }

        [Fact]
        public async Task GetByClientIdAsync_ShouldReturnNull_WhenClientDoesNotExist()
        {
            // Arrange
            var clientId = "non-existent-client-id";
            _mockCollection.Setup(c => c.Find(It.IsAny<FilterDefinition<ClientAuthData>>())
                .FirstOrDefaultAsync(default)).ReturnsAsync((ClientAuthData)null);

            // Act
            var result = await _authRepository.GetByClientIdAsync(clientId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByEmailAsync_ShouldReturnClient_WhenEmailExists()
        {
            // Arrange
            var email = "test@example.com";
            var expectedClient = new ClientAuthData { ClientId = "client-id", Email = email };
            _mockCollection.Setup(c => c.Find(It.IsAny<FilterDefinition<ClientAuthData>>())
                .FirstOrDefaultAsync(default)).ReturnsAsync(expectedClient);

            // Act
            var result = await _authRepository.GetByEmailAsync(email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(email, result.Email);
        }

        [Fact]
        public async Task CreateAsync_ShouldInsertClient_WhenClientIsNew()
        {
            // Arrange
            var client = new ClientAuthData { ClientId = "new-client-id", Email = "new@example.com" };

            // Act
            await _authRepository.CreateAsync(client);

            // Assert
            _mockCollection.Verify(c => c.InsertOneAsync(client, It.IsAny<InsertOneOptions>(), default), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReplaceClient_WhenClientExists()
        {
            // Arrange
            var clientId = "existing-client-id";
            var updatedClient = new ClientAuthData { ClientId = clientId, Email = "updated@example.com" };

            // Act
            await _authRepository.UpdateAsync(clientId, updatedClient);

            // Assert
            _mockCollection.Verify(c => c.ReplaceOneAsync(It.IsAny<FilterDefinition<ClientAuthData>>(), updatedClient, It.IsAny<ReplaceOptions>(), default), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveClient_WhenClientExists()
        {
            // Arrange
            var clientId = "existing-client-id";

            // Act
            await _authRepository.DeleteAsync(clientId);

            // Assert
            _mockCollection.Verify(c => c.DeleteOneAsync(It.IsAny<FilterDefinition<ClientAuthData>>(), default), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnListOfClients()
        {
            // Arrange
            var clients = new List<ClientAuthData>
            {
                new ClientAuthData { ClientId = "client1", Email = "client1@example.com" },
                new ClientAuthData { ClientId = "client2", Email = "client2@example.com" }
            };
            _mockCollection.Setup(c => c.Find(It.IsAny<FilterDefinition<ClientAuthData>>()).ToListAsync(default)).ReturnsAsync(clients);

            // Act
            var result = await _authRepository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }
    }
}
