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
            // 1. Leer la configuración desde Web.config <appSettings>
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
                // Manejar error: Lanzar excepción, loggear, o devolver null/string vacío
                throw new ConfigurationErrorsException("Las claves JWT (Secret, Audience, Issuer) no están configuradas correctamente en Web.config.");
            }

            // 2. Crear la llave de seguridad simétrica
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            // 3. Crear las credenciales de firma
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            // 4. Crear los claims (información dentro del token)
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, username), 
                new Claim(ClaimTypes.Name, username),           
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Identificador único del token
            };

            // 5. Crear el token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Audience = audienceToken,
                Issuer = issuerToken,
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                SigningCredentials = signingCredentials
            };

            // 6. Generar el token como string
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtTokenString = tokenHandler.WriteToken(token);

            return jwtTokenString;
        }
    }
}