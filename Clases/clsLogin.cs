using System;
using System.Linq;
using TorneosITM_API.Models; 

namespace TorneosITM_API.Clases
{
    public class clsLogin
    {
        public LoginModel login { get; set; }

        // Método principal que realiza la validación y genera el token
        public LoginResponse Ingresar()
        {
            var respuesta = new LoginResponse();
            try
            {
                if (this.login == null || string.IsNullOrEmpty(this.login.Usuario) || string.IsNullOrEmpty(this.login.Clave))
                {
                    respuesta.Mensaje = "Debe proporcionar Usuario y Clave.";
                    respuesta.Autenticado = false;
                    return respuesta;
                }

                // Creamos el contexto de la base de datos dentro de un using para asegurar su disposición
                using (DBExamenEntities db = new DBExamenEntities())
                {
                    // 1. Buscamos al administrador por el nombre de usuario
                    var administrador = db.AdministradorITMs
                                          .FirstOrDefault(a => a.Usuario == this.login.Usuario);

                    // 2. Verificamos si el administrador existe
                    if (administrador == null)
                    {
                        respuesta.Mensaje = "Usuario no encontrado.";
                        respuesta.Autenticado = false;
                    }
                    else
                    {
                        // 3. Verificamos si la clave coincide (comparación directa en texto plano)
                        if (administrador.Clave == this.login.Clave)
                        {
                            // ¡Autenticación exitosa!
                            // 4. Generamos el token JWT
                            string tokenGenerado = TokenGenerator.GenerateTokenJwt(administrador.Usuario);

                            // 5. Preparamos la respuesta exitosa
                            respuesta.Autenticado = true;
                            respuesta.Usuario = administrador.Usuario;
                            respuesta.Token = tokenGenerado;
                            respuesta.Mensaje = "Autenticación exitosa.";
                        }
                        else
                        {
                            // La clave no coincide
                            respuesta.Mensaje = "Clave incorrecta.";
                            respuesta.Autenticado = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de cualquier otro error inesperado
                // En un caso real, sería bueno loggear el ex.ToString() para más detalles
                respuesta.Mensaje = "Error durante la autenticación: " + ex.Message;
                respuesta.Autenticado = false;
                // Asegurarse de que el token esté vacío en caso de error
                respuesta.Token = string.Empty;
                respuesta.Usuario = string.Empty;
            }
            // Devolvemos el objeto de respuesta (éxito o fallo)
            return respuesta;
        }
    }
}