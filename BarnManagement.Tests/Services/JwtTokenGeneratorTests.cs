using BarnManagement.Business.Services;
using BarnManagement.Core.Entities;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Xunit;

namespace BarnManagement.Tests.Services
{
    public class JwtTokenGeneratorTests
    {
        private readonly Mock<IConfiguration> _configMock;
        private readonly JwtTokenGenerator _generator;

        public JwtTokenGeneratorTests()
        {
            _configMock = new Mock<IConfiguration>();
            _configMock.Setup(x => x["JwtSettings:Secret"]).Returns("very_long_secret_key_for_testing_purposes_123!");
            _configMock.Setup(x => x["JwtSettings:Issuer"]).Returns("TestIssuer");
            _configMock.Setup(x => x["JwtSettings:Audience"]).Returns("TestAudience");

            _generator = new JwtTokenGenerator(_configMock.Object);
        }

        [Fact]
        public void GenerateToken_ShouldReturnValidJwtString()
        {
            // Arrange
            var user = new User 
            { 
                Id = Guid.NewGuid(), 
                Email = "test@jwt.com", 
                Username = "jwtuser" 
            };

            // Act
            var token = _generator.GenerateToken(user);

            // Assert
            token.Should().NotBeNullOrEmpty();
            
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            
            jsonToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value.Should().Be(user.Id.ToString());
            jsonToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value.Should().Be(user.Email);
            jsonToken.Issuer.Should().Be("TestIssuer");
        }
    }
}
