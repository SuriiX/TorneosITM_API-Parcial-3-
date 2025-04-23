using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims; 
using System.Text; 
using System.Threading;
using System.Threading.Tasks;
using System.Web; 

namespace TorneosITM_API.Clases 
{
    public class TokenValidationHandler : DelegatingHandler
    {
       
        private static bool TryRetrieveToken(HttpRequestMessage request, out string token)
        {
            token = null;
            IEnumerable<string> authzHeaders;
           
            if (!request.Headers.TryGetValues("Authorization", out authzHeaders) || authzHeaders.Count() > 1)
            {
                
                return false;
            }
            var bearerToken = authzHeaders.ElementAt(0);
            
            token = bearerToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) ? bearerToken.Substring(7) : null;
            return !string.IsNullOrEmpty(token);
        }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpStatusCode statusCode;
            string token;

            
            if (!TryRetrieveToken(request, out token))
            {
                
                return Task<HttpResponseMessage>.Factory.StartNew(() => new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "Authorization header is missing or invalid." });
            }

            try
            {
                
                var secretKey = ConfigurationManager.AppSettings["JWT_SECRET_KEY"];
                var audienceToken = ConfigurationManager.AppSettings["JWT_AUDIENCE_TOKEN"];
                var issuerToken = ConfigurationManager.AppSettings["JWT_ISSUER_TOKEN"];

            
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

                
                var validationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = securityKey,  

                    ValidateIssuer = true,         
                    ValidIssuer = issuerToken,       

                    ValidateAudience = true,      
                    ValidAudience = audienceToken,   

                    ValidateLifetime = true,       
                    ClockSkew = TimeSpan.Zero,    
                                                   
                };

                // Validamos el token
                var tokenHandler = new JwtSecurityTokenHandler();
                SecurityToken securityToken; 
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);

                Thread.CurrentPrincipal = principal;
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.User = principal;
                }

                
                return base.SendAsync(request, cancellationToken);

            }
            catch (SecurityTokenValidationException stve)
            {
                
                statusCode = HttpStatusCode.Unauthorized;
                
            }
            catch (Exception ex)
            {
                
                statusCode = HttpStatusCode.InternalServerError;
              
            }

            
            return Task<HttpResponseMessage>.Factory.StartNew(() => new HttpResponseMessage(statusCode));
        }

    }
}