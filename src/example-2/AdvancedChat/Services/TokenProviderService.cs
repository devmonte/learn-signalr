using AdvancedChat.Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedChat.Services
{
    public class TokenProviderService : ITokenProviderService
    {
        private readonly IConfiguration _configuration;

        public TokenProviderService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(UserDto user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["SecretKey"]);
            //var tokenDescriptor = new SecurityTokenDescriptor
            //{
            //    Subject = new ClaimsIdentity(new Claim[]
            //    {
            //        new Claim(ClaimTypes.NameIdentifier, user.Name),
            //        new Claim("Group", user.Group)
            //    }),
            //    Issuer = "ExampleApp",
            //    Expires = DateTime.UtcNow.AddSeconds(30),
            //    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            //};

            //var token = tokenHandler.CreateToken(tokenDescriptor);
            //return tokenHandler.WriteToken(token);

            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, user.Name) };
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken("SignalRTestServer", "SignalRTests", claims, expires: DateTime.UtcNow.AddSeconds(30), signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256));
            return tokenHandler.WriteToken(token);
        }
    }
}
