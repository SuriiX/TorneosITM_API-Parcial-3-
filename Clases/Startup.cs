using Microsoft.IdentityModel.Tokens; 
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt; 
using Owin; 
using System;
using System.Configuration; 
using System.Text; 
using System.Web.Http; 


[assembly: OwinStartup(typeof(TorneosITM_API.Startup))] 

namespace TorneosITM_API
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            

            
            var secretKey = ConfigurationManager.AppSettings["JWT_SECRET_KEY"];
            var audienceToken = ConfigurationManager.AppSettings["JWT_AUDIENCE_TOKEN"];
            var issuerToken = ConfigurationManager.AppSettings["JWT_ISSUER_TOKEN"];

            
            if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(audienceToken) || string.IsNullOrEmpty(issuerToken))
            {
                throw new ConfigurationErrorsException("Las claves JWT (Secret, Audience, Issuer) no están configuradas correctamente en Web.config para OWIN Startup.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

           
            var jwtBearerOptions = new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true, // Validar la firma basada en la clave
                    IssuerSigningKey = securityKey,

                    ValidateIssuer = true,         // Validar el emisor
                    ValidIssuer = issuerToken,

                    ValidateAudience = true,       // Validar la audiencia
                    ValidAudience = audienceToken,

                    ValidateLifetime = true,     
                }
                
            };

         
            app.UseJwtBearerAuthentication(jwtBearerOptions);

         
        }
    }
}