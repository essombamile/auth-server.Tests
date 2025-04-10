using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Xunit;
using System;
using auth_server.Models;

namespace auth_server.Tests
{
    public class ClientAuthDataTests
    {
        [Fact]
        public void ShouldSerializeClientAuthDataCorrectly()
        {
            // Arrange
            var clientAuthData = new ClientAuthData
            {
                Id = "507f1f77bcf86cd799439011",
                ClientId = "client123",
                BankId = "bank456",
                ClientName = "John Doe",
                PasswordHash = "hashedpassword",
                CardNumber = "1234567812345678",
                NationalRegistryNumber = "123456789",
                BirthDate = new DateTime(1990, 1, 1),
                AgeAtRequest = 30,
                Gender = "Male",
                RequestDateTime = DateTime.UtcNow,
                ChallengeSent = "some-challenge",
                Email = "john.doe@example.com",
                SaltSent = "randomsalt"
            };

            // Act
            var bsonDocument = clientAuthData.ToBsonDocument();

            // Assert
            Assert.Equal(clientAuthData.Id, bsonDocument["_id"].AsString);
            Assert.Equal(clientAuthData.ClientId, bsonDocument["clientId"].AsString);
            Assert.Equal(clientAuthData.BankId, bsonDocument["bankId"].AsString);
            Assert.Equal(clientAuthData.ClientName, bsonDocument["clientName"].AsString);
            Assert.Equal(clientAuthData.PasswordHash, bsonDocument["passwordHash"].AsString);
            Assert.Equal(clientAuthData.CardNumber, bsonDocument["cardNumber"].AsString);
            Assert.Equal(clientAuthData.NationalRegistryNumber, bsonDocument["nationalRegistryNumber"].AsString);
            Assert.Equal(clientAuthData.BirthDate, bsonDocument["birthDate"].ToUniversalTime());
            Assert.Equal(clientAuthData.AgeAtRequest, bsonDocument["ageAtRequest"].AsInt32);
            Assert.Equal(clientAuthData.Gender, bsonDocument["gender"].AsString);
            Assert.Equal(clientAuthData.RequestDateTime, bsonDocument["requestDateTime"].ToUniversalTime());
            Assert.Equal(clientAuthData.ChallengeSent, bsonDocument["challengeSent"].AsString);
            Assert.Equal(clientAuthData.Email, bsonDocument["email"].AsString);
            Assert.Equal(clientAuthData.SaltSent, bsonDocument["saltSent"].AsString);
        }

        [Fact]
        public void ShouldDeserializeClientAuthDataCorrectly()
        {
            // Arrange
            var bsonDocument = new BsonDocument
            {
                { "_id", "507f1f77bcf86cd799439011" },
                { "clientId", "client123" },
                { "bankId", "bank456" },
                { "clientName", "John Doe" },
                { "passwordHash", "hashedpassword" },
                { "cardNumber", "1234567812345678" },
                { "nationalRegistryNumber", "123456789" },
                { "birthDate", new BsonDateTime(new DateTime(1990, 1, 1)) },
                { "ageAtRequest", 30 },
                { "gender", "Male" },
                { "requestDateTime", new BsonDateTime(DateTime.UtcNow) },
                { "challengeSent", "some-challenge" },
                { "email", "john.doe@example.com" },
                { "saltSent", "randomsalt" }
            };

            // Act
            var clientAuthData = BsonSerializer.Deserialize<ClientAuthData>(bsonDocument);

            // Assert
            Assert.Equal("507f1f77bcf86cd799439011", clientAuthData.Id);
            Assert.Equal("client123", clientAuthData.ClientId);
            Assert.Equal("bank456", clientAuthData.BankId);
            Assert.Equal("John Doe", clientAuthData.ClientName);
            Assert.Equal("hashedpassword", clientAuthData.PasswordHash);
            Assert.Equal("1234567812345678", clientAuthData.CardNumber);
            Assert.Equal("123456789", clientAuthData.NationalRegistryNumber);
            Assert.Equal(new DateTime(1990, 1, 1), clientAuthData.BirthDate);
            Assert.Equal(30, clientAuthData.AgeAtRequest);
            Assert.Equal("Male", clientAuthData.Gender);
            Assert.Equal("some-challenge", clientAuthData.ChallengeSent);
            Assert.Equal("john.doe@example.com", clientAuthData.Email);
            Assert.Equal("randomsalt", clientAuthData.SaltSent);
        }
    }
}
