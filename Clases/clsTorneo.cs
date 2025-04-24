using System;
using System.Collections.Generic;
using System.Data.Entity; // Asegúrate de tener este using para .Include()
using System.Linq;
using TorneosITM_API.Models;

namespace TorneosITM_API.Clases
{
    public class clsTorneo
    {
        // --- Operaciones CRUD ---
        // (Los métodos Insertar, Actualizar, Eliminar no necesitan cambios aquí
        // ya que no devuelven el objeto Torneo completo para serialización)

        public string Insertar(Torneo nuevoTorneo)
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
                // Considera loggear el detalle del error ex para mejor diagnóstico
                return "Error al registrar el torneo: " + ex.Message;
            }
        }

        public string Actualizar(Torneo torneoActualizado)
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

                    // Actualizamos las propiedades
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



        public Torneo ConsultarPorId(int idTorneo)
        {
            try
            {
                using (DBExamenEntities db = new DBExamenEntities())
                {
                    return db.Torneos
                             .Include(t => t.AdministradorITM)
                             .FirstOrDefault(t => t.idTorneos == idTorneo);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<Torneo> ConsultarTodos()
        {
            try
            {
                using (DBExamenEntities db = new DBExamenEntities())
                {
                    return db.Torneos
                             .Include(t => t.AdministradorITM)
                             .OrderBy(t => t.FechaTorneo)
                             .ThenBy(t => t.NombreTorneo)
                             .ToList();
                }
            }
            catch (Exception)
            {
                return new List<Torneo>();
            }
        }
    }
}
