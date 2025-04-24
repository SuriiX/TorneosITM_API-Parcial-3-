using Microsoft.IdentityModel.Tokens;
using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text; 

namespace TorneosITM_API.Clases 
{
    public static class TokenGenerator
    {
        public static string GenerateTokenJwt(string username)
        {
            var secretKey = ConfigurationManager.AppSettings["JWT_SECRET_KEY"];
            var audienceToken = ConfigurationManager.AppSettings["JWT_AUDIENCE_TOKEN"];
            var issuerToken = ConfigurationManager.AppSettings["JWT_ISSUER_TOKEN"];

            int expireMinutes;
            if (!int.TryParse(ConfigurationManager.AppSettings["JWT_EXPIRE_MINUTES"], out expireMinutes))
            {
                expireMinutes = 60; 
            }

            if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(audienceToken) || string.IsNullOrEmpty(issuerToken))
            {
                throw new ConfigurationErrorsException("Las claves JWT (Secret, Audience, Issuer) no están configuradas correctamente en Web.config.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, username), 
                new Claim(ClaimTypes.Name, username), 
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) 
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Audience = audienceToken,
                Issuer = issuerToken,
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                SigningCredentials = signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtTokenString = tokenHandler.WriteToken(token);

            return jwtTokenString;
        }
    }
}