using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TorneosITM_API.Models
{
    public class LoginResponse
    {
        public string Usuario { get; set; }
        public bool Autenticado { get; set; }
        public string Token { get; set; }
        public string Mensaje { get; set; }

        // Constructor opcional para inicializar valores por defecto
        public LoginResponse()
        {
            Usuario = string.Empty;
            Autenticado = false;
            Token = string.Empty;
            Mensaje = string.Empty;
        }
    }
}