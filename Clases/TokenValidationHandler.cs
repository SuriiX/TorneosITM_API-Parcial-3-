using Microsoft.IdentityModel.Tokens;
using System;
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
using System.Web.Http;
using System.Web.Http.Controllers;
using Microsoft.Owin; 

namespace TorneosITM_API.Clases
{
    public class TokenValidationHandler : DelegatingHandler
    {
        private static readonly PathString LoginPath = new PathString("/api/Login/Ingresar"); 

        private static bool TryRetrieveToken(HttpRequestMessage request, out string token)
        {
            token = null;
            if (!request.Headers.TryGetValues("Authorization", out var authzHeaders) || authzHeaders.Count() > 1) { return false; }
            var bearerToken = authzHeaders.ElementAt(0);
            token = bearerToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) ? bearerToken.Substring(7) : bearerToken;
            return !string.IsNullOrEmpty(token);
        }

        private static TokenValidationParameters GetValidationParameters()
        {
            var secretKey = ConfigurationManager.AppSettings["JWT_SECRET_KEY"];
            var audienceToken = ConfigurationManager.AppSettings["JWT_AUDIENCE_TOKEN"];
            var issuerToken = ConfigurationManager.AppSettings["JWT_ISSUER_TOKEN"];

            if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(audienceToken) || string.IsNullOrEmpty(issuerToken))
            {
                System.Diagnostics.Debug.WriteLine("ERROR CRÍTICO: Claves JWT no configuradas en Web.config para TokenValidationHandler.");
                throw new ConfigurationErrorsException("Las claves JWT (Secret, Audience, Issuer) no están configuradas para la validación.");
            }

            return new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ValidateIssuer = true,
                ValidIssuer = issuerToken,
                ValidateAudience = true,
                ValidAudience = audienceToken,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        }
        private bool ShouldBypassTokenValidation(HttpRequestMessage request)
        {
            if (request.RequestUri != null && request.GetOwinContext().Request.Path.Equals(LoginPath)) 
            {
                System.Diagnostics.Debug.WriteLine($"TokenValidationHandler: Ruta {request.RequestUri.AbsolutePath} coincide con LoginPath explícito. Saltando validación.");
                return true;
            }
            HttpActionDescriptor actionDescriptor = null;
            try { actionDescriptor = request.GetActionDescriptor(); }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"TokenValidationHandler: Excepción obteniendo ActionDescriptor: {ex.Message}"); }

            if (actionDescriptor != null)
            {
                bool isAnonymous = actionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any() ||
                                   actionDescriptor.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
                if (isAnonymous) System.Diagnostics.Debug.WriteLine($"TokenValidationHandler: Ruta {request.RequestUri?.AbsolutePath} tiene [AllowAnonymous]. Saltando validación.");
                return isAnonymous; // Devuelve true si tiene el atributo
            }
            System.Diagnostics.Debug.WriteLine($"TokenValidationHandler: Ruta {request.RequestUri?.AbsolutePath} NO es anónima. Procediendo a validar token.");
            return false;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpStatusCode statusCode;
            string token;
            if (ShouldBypassTokenValidation(request))
            {
                return await base.SendAsync(request, cancellationToken);
            }
            if (!TryRetrieveToken(request, out token))
            {
                statusCode = HttpStatusCode.Unauthorized;
                return request.CreateResponse(statusCode, new { Message = "Token de autorización no proporcionado o inválido." });
            }
            try
            {
                var validationParameters = GetValidationParameters();
                var tokenHandler = new JwtSecurityTokenHandler();
                SecurityToken validatedToken;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

                Thread.CurrentPrincipal = principal;
                if (HttpContext.Current != null) { HttpContext.Current.User = principal; }
                request.GetRequestContext().Principal = principal;

                return await base.SendAsync(request, cancellationToken);
            }
            catch (SecurityTokenValidationException ex)
            {
                statusCode = HttpStatusCode.Unauthorized;
                System.Diagnostics.Debug.WriteLine($"TokenValidationHandler: Token inválido - {ex.Message}");
                return request.CreateResponse(statusCode, new { Message = "Token inválido o expirado.", Details = ex.Message });
            }
            catch (Exception ex)
            {
                statusCode = HttpStatusCode.InternalServerError;
                System.Diagnostics.Debug.WriteLine($"TokenValidationHandler: Error interno - {ex.ToString()}");
                return request.CreateResponse(statusCode, new { Message = "Error interno del servidor al validar el token." });
            }
        }

        public bool LifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters)
        {
            if (expires != null) { return DateTime.UtcNow < expires; }
            return false;
        }
    }
}