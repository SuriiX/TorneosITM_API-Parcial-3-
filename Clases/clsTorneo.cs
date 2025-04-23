using System;
using System.Collections.Generic;
using System.Data.Entity; 
using System.Linq;
using TorneosITM_API.Models;

namespace TorneosITM_API.Clases 
{
    public class clsTorneo
    {
        // --- Operaciones CRUD ---

        public string Insertar(Torneos nuevoTorneo)
        {
            try
            {
                using (DBExamenEntities db = new DBExamenEntities())
                {
                    
                    var adminExists = db.AdministradorITMs.Any(a => a.idAministradorITM == nuevoTorneo.idAdministradorITM);
                    if (!adminExists)
                    {
                        return "Error: El administrador especificado no existe.";
                    }

                    db.Torneos.Add(nuevoTorneo);
                    db.SaveChanges();
                    return "Torneo registrado correctamente. ID: " + nuevoTorneo.idTorneos;
                }
            }
            catch (Exception ex)
            {
                
                return "Error al registrar el torneo: " + ex.Message;
            }
        }

        public string Actualizar(Torneos torneoActualizado)
        {
            try
            {
                using (DBExamenEntities db = new DBExamenEntities())
                {
                    
                    var torneoExistente = db.Torneos.Find(torneoActualizado.idTorneos);

                    if (torneoExistente == null)
                    {
                        return "Error: El torneo que intenta actualizar no existe (ID: " + torneoActualizado.idTorneos + ").";
                    }

                    
                    var adminExists = db.AdministradorITMs.Any(a => a.idAministradorITM == torneoActualizado.idAdministradorITM);
                    if (!adminExists)
                    {
                        return "Error: El nuevo administrador especificado no existe.";
                    }

                    // Actualizamos las propiedades del torneo existente con los valores nuevos
                    
                    torneoExistente.idAdministradorITM = torneoActualizado.idAdministradorITM;
                    torneoExistente.TipoTorneo = torneoActualizado.TipoTorneo;
                    torneoExistente.NombreTorneo = torneoActualizado.NombreTorneo;
                    torneoExistente.NombreEquipo = torneoActualizado.NombreEquipo;
                    torneoExistente.ValorInscripcion = torneoActualizado.ValorInscripcion;
                    torneoExistente.FechaTorneo = torneoActualizado.FechaTorneo;
                    torneoExistente.Integrantes = torneoActualizado.Integrantes;

                    
                    db.Entry(torneoExistente).State = EntityState.Modified;

                    db.SaveChanges();
                    return "Torneo actualizado correctamente (ID: " + torneoActualizado.idTorneos + ").";
                }
            }
            catch (Exception ex)
            {
                return "Error al actualizar el torneo: " + ex.Message;
            }
        }

        public string Eliminar(int idTorneo)
        {
            try
            {
                using (DBExamenEntities db = new DBExamenEntities())
                {
                    var torneoAEliminar = db.Torneos.Find(idTorneo);

                    if (torneoAEliminar == null)
                    {
                        return "Error: El torneo que intenta eliminar no existe (ID: " + idTorneo + ").";
                    }

                    db.Torneos.Remove(torneoAEliminar);
                    db.SaveChanges();
                    return "Torneo eliminado correctamente (ID: " + idTorneo + ").";
                }
            }
            catch (Exception ex)
            {
                
                return "Error al eliminar el torneo: " + ex.Message;
            }
        }

        // --- Consultas ---

        public Torneos ConsultarPorId(int idTorneo)
        {
            try
            {
                using (DBExamenEntities db = new DBExamenEntities())
                {
                   
                    return db.Torneos.Find(idTorneo);
                }
            }
            catch (Exception)
            {
                
                return null; 
            }
        }

        public List<Torneos> ConsultarTodos()
        {
            try
            {
                using (DBExamenEntities db = new DBExamenEntities())
                {
                    return db.Torneos.OrderBy(t => t.FechaTorneo).ThenBy(t => t.NombreTorneo).ToList();
                }
            }
            catch (Exception)
            {
                
                return new List<Torneos>(); 
            }
        }

        public List<Torneos> ConsultarPorTipo(string tipo)
        {
            try
            {
                using (DBExamenEntities db = new DBExamenEntities())
                {
                    
                    return db.Torneos
                             .Where(t => t.TipoTorneo.ToLower() == tipo.ToLower())
                             .OrderBy(t => t.FechaTorneo).ThenBy(t => t.NombreTorneo)
                             .ToList();
                }
            }
            catch (Exception)
            {
                return new List<Torneos>();
            }
        }

        public List<Torneos> ConsultarPorNombre(string nombre)
        {
            try
            {
                using (DBExamenEntities db = new DBExamenEntities())
                {
                    
                    return db.Torneos
                             .Where(t => t.NombreTorneo.ToLower().Contains(nombre.ToLower()))
                             .OrderBy(t => t.FechaTorneo).ThenBy(t => t.NombreTorneo)
                             .ToList();
                }
            }
            catch (Exception)
            {
                return new List<Torneos>();
            }
        }

        public List<Torneos> ConsultarPorFecha(DateTime fecha)
        {
            try
            {
                using (DBExamenEntities db = new DBExamenEntities())
                {
                    
                    return db.Torneos
                             .Where(t => DbFunctions.TruncateTime(t.FechaTorneo) == DbFunctions.TruncateTime(fecha))
                            
                             .OrderBy(t => t.NombreTorneo)
                             .ToList();
                }
            }
            catch (Exception)
            {
                return new List<Torneos>();
            }
        }
    }
}