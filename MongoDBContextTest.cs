using auth_server.Data;
using auth_server.Models;
using Microsoft.Extensions.Configuration;
using Moq;
using MongoDB.Driver;
using Xunit;
using auth_server.Data;
using auth_server.Models;

namespace auth_server.Tests
{
    public class MongoDbContextTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IMongoClient> _mockMongoClient;
        private readonly Mock<IMongoDatabase> _mockDatabase;
        private readonly Mock<IMongoCollection<ClientAuthData>> _mockCollection;
        private readonly MongoDbContext _mongoDbContext;

        public MongoDbContextTests()
        {
            // Setup du mock pour IConfiguration
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(config => config.GetConnectionString("MongoDB")).Returns("mongodb://localhost:27017");

            // Setup du mock pour MongoClient et MongoDatabase
            _mockMongoClient = new Mock<IMongoClient>();
            _mockDatabase = new Mock<IMongoDatabase>();
            _mockCollection = new Mock<IMongoCollection<ClientAuthData>>();

            // Setup pour le retour de GetDatabase
            _mockMongoClient.Setup(client => client.GetDatabase(It.IsAny<string>())).Returns(_mockDatabase.Object);
            _mockDatabase.Setup(db => db.GetCollection<ClientAuthData>(It.IsAny<string>())).Returns(_mockCollection.Object);

            // Instanciation de MongoDbContext avec le mock de IConfiguration
            _mongoDbContext = new MongoDbContext(_mockConfiguration.Object);
        }

        [Fact]
        public void Constructor_ShouldInitializeDatabase()
        {
            // Arrange & Act
            var database = _mongoDbContext.GetType().GetField("_database", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(_mongoDbContext);

            // Assert
            Assert.NotNull(database);
        }

        [Fact]
        public void Clients_ShouldReturnMockedCollection()
        {
            // Act
            var collection = _mongoDbContext.Clients;

            // Assert
            Assert.NotNull(collection);
            Assert.IsType<Mock<IMongoCollection<ClientAuthData>>>(collection);
        }

        [Fact]
        public void Constructor_ShouldUseCorrectConnectionString()
        {
            // Act
            _mongoDbContext.Clients;

            // Assert
            _mockConfiguration.Verify(config => config.GetConnectionString("MongoDB"), Times.Once);
            _mockMongoClient.Verify(client => client.GetDatabase("AuthDatabase"), Times.Once);
        }
    }
}
