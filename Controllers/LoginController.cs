using System.Web.Http;
using TorneosITM_API.Clases;
using TorneosITM_API.Models;

namespace TorneosITM_API.Controllers 
{
    [RoutePrefix("api/Login")] 
    [AllowAnonymous] 
    public class LoginController : ApiController
    {
        [HttpPost] 
        [Route("Ingresar")] // Define la ruta específica: api/Login/Ingresar
        public IHttpActionResult Ingresar([FromBody] LoginModel login) 
        {
            // Validación básica del modelo recibido
            if (login == null)
            {
                return BadRequest("Solicitud inválida. El cuerpo no puede estar vacío.");
            }
            // Valida si el modelo cumple las reglas (si tuvieras DataAnnotations en LoginModel)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            clsLogin _loginLogic = new clsLogin();
            _loginLogic.login = login;
            // Ejecutamos el método de ingreso y obtenemos la respuesta
            LoginResponse respuesta = _loginLogic.Ingresar();
            // Devolvemos la respuesta. Usamos Ok() para que el código HTTP sea 200 OK.
            // El cliente deberá verificar la propiedad 'Autenticado' en la respuesta JSON.
            return Ok(respuesta);
        }
    }
}