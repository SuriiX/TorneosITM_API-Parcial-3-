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
        [Route("Ingresar")]
        public IHttpActionResult Ingresar([FromBody] LoginModel login) 
        {
            if (login == null)
            {
                return BadRequest("Solicitud inválida. Cuerpo JSON incorrecto.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            clsLogin _loginLogic = new clsLogin();
            _loginLogic.login = login;

            LoginResponse respuesta = _loginLogic.Ingresar(); 

            if (respuesta.Autenticado)
            {
                return Ok(respuesta);
            }
            else
            {
                return BadRequest(respuesta.Mensaje ?? "Credenciales inválidas.");
            }
        }
    }
}