using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ProgramaYA.Models
{
    public class Usuario: IdentityUser
    {
        public string? Nombres {get;set;}
        public string? Apellidos {get;set;}
        public string? DNI {get;set;}
        public string? Correo {get;set;}
        public string? Contrasena {get;set;}
        public string? Celular {get;set;}
    }
}