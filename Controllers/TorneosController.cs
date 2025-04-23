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
        public IHttpActionResult Registrar([FromBody] Torneos torneo)
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
        public IHttpActionResult Actualizar([FromBody] Torneos torneo)
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
            List<Torneos> torneos = _logic.ConsultarTodos();
           
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
            Torneos torneo = _logic.ConsultarPorId(id);
            if (torneo == null)
            {
                return NotFound();
            }
            return Ok(torneo);
        }

        // GET api/Torneos/ConsultarPorTipo/Futbol
        [HttpGet]
        [Route("ConsultarPorTipo/{tipo}")] 
        public IHttpActionResult ConsultarPorTipo(string tipo)
        {
            if (string.IsNullOrWhiteSpace(tipo))
            {
                return BadRequest("El tipo de torneo no puede estar vacío.");
            }
            List<Torneos> torneos = _logic.ConsultarPorTipo(tipo);
            return Ok(torneos); 
        }

        // GET api/Torneos/ConsultarPorNombre/Campeonato%20Verano
        [HttpGet]
        [Route("ConsultarPorNombre/{nombre}")] 
        public IHttpActionResult ConsultarPorNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                return BadRequest("El nombre del torneo no puede estar vacío.");
            }
            List<Torneos> torneos = _logic.ConsultarPorNombre(nombre);
            return Ok(torneos); 
        }

        // GET api/Torneos/ConsultarPorFecha/2025-10-20
        [HttpGet]
        [Route("ConsultarPorFecha/{fecha:datetime}")] 
        public IHttpActionResult ConsultarPorFecha(DateTime fecha)
        {
            
            List<Torneos> torneos = _logic.ConsultarPorFecha(fecha);
            return Ok(torneos);
        }
    }
}