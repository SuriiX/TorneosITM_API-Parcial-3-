using System;
using System.Collections.Generic;
using System.Web.Http;
using TorneosITM_API.Clases;
using TorneosITM_API.Models; 

namespace TorneosITM_API.Controllers 
{
    [Authorize] 
    [RoutePrefix("api/Torneos")]
    public class TorneosController : ApiController
    {
        private clsTorneo _logic; 

        
        public TorneosController()
        {
            _logic = new clsTorneo();
        }

        // POST api/Torneos/Registrar
        [HttpPost]
        [Route("Registrar")]
        public IHttpActionResult Registrar([FromBody] Torneo torneo)
        {
            if (torneo == null)
            {
                return BadRequest("Datos del torneo inválidos.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string resultado = _logic.Insertar(torneo);

            
            if (resultado.StartsWith("Error:"))
            {
                return BadRequest(resultado);
            }
            return Ok(resultado);
        }

        // PUT api/Torneos/Actualizar
        [HttpPut]
        [Route("Actualizar")]
        public IHttpActionResult Actualizar([FromBody] Torneo torneo)
        {
            if (torneo == null || torneo.idTorneos <= 0) 
            {
                return BadRequest("Datos del torneo inválidos o falta ID.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string resultado = _logic.Actualizar(torneo);

            if (resultado.StartsWith("Error: El torneo que intenta actualizar no existe"))
            {
                return NotFound(); 
            }
            if (resultado.StartsWith("Error:"))
            {
                return BadRequest(resultado); 
            }
            return Ok(resultado); 
        }

       
        [HttpDelete]
        [Route("Eliminar/{id:int}")] 
        public IHttpActionResult Eliminar(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID de torneo inválido.");
            }

            string resultado = _logic.Eliminar(id);

            if (resultado.StartsWith("Error: El torneo que intenta eliminar no existe"))
            {
                return NotFound(); 
            }
            if (resultado.StartsWith("Error:"))
            {
                return BadRequest(resultado); 
            }
            return Ok(resultado);
        }

       
        [HttpGet]
        [Route("ConsultarTodos")]
        public IHttpActionResult ConsultarTodos()
        {
            List<Torneo> torneos = _logic.ConsultarTodos();
           
            return Ok(torneos);
        }

        // GET api/Torneos/ConsultarPorId/5
        [HttpGet]
        [Route("ConsultarPorId/{id:int}")]
        public IHttpActionResult ConsultarPorId(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID de torneo inválido.");
            }
            Torneo torneo = _logic.ConsultarPorId(id);
            if (torneo == null)
            {
                return NotFound();
            }
            return Ok(torneo);
        }

    }
}