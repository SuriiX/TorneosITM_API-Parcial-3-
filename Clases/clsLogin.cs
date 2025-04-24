using System;
using System.Linq;
using TorneosITM_API.Models; // Namespace correcto

namespace TorneosITM_API.Clases // Namespace correcto
{
    public class clsLogin
    {
        // Propiedad para recibir los datos del controlador
        public LoginModel login { get; set; } // Usa el LoginModel original

        // Método original que valida contra DBExamenEntities
        public LoginResponse Ingresar() // Usa el LoginResponse original
        {
            var respuesta = new LoginResponse();
            try
            {
                // Validaciones básicas de entrada
                if (this.login == null || string.IsNullOrEmpty(this.login.Usuario) || string.IsNullOrEmpty(this.login.Clave))
                {
                    respuesta.Mensaje = "Debe proporcionar Usuario y Clave.";
                    respuesta.Autenticado = false;
                    return respuesta;
                }

                // Usa el DbContext correcto
                using (DBExamenEntities db = new DBExamenEntities())
                {
                    // Busca por usuario en AdministradorITM
                    var administrador = db.AdministradorITMs
                                          .FirstOrDefault(a => a.Usuario == this.login.Usuario);

                    if (administrador == null)
                    {
                        respuesta.Mensaje = "Usuario no encontrado.";
                        respuesta.Autenticado = false;
                    }
                    else
                    {
                        // IMPORTANTE: Comparación directa de clave (sin hashing)
                        if (administrador.Clave == this.login.Clave)
                        {
                            // Autenticación exitosa
                            // Llama al TokenGenerator original de este proyecto
                            string tokenGenerado = TokenGenerator.GenerateTokenJwt(administrador.Usuario);

                            respuesta.Autenticado = true;
                            respuesta.Usuario = administrador.Usuario;
                            respuesta.Token = tokenGenerado;
                            respuesta.Mensaje = "Autenticación exitosa.";
                        }
                        else
                        {
                            respuesta.Mensaje = "Clave incorrecta.";
                            respuesta.Autenticado = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Loggear ex.ToString() sería útil
                respuesta.Mensaje = "Error durante la autenticación: " + ex.Message;
                respuesta.Autenticado = false;
                respuesta.Token = string.Empty;
                respuesta.Usuario = string.Empty;
            }
            return respuesta;
        }
    }
}