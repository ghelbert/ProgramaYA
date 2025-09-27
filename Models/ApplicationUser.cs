using Microsoft.AspNetCore.Identity;

namespace ProgramaYA.Models
{
    public class ApplicationUser : IdentityUser
    {    
    public string? Nombres { get; set; }
    public string? Apellidos { get; set; }
    public string? DNI { get; set; }
    public string? Celular { get; set; }
    public List<Suscripcion>? Suscripciones { get; set; }
    public List<Curso>? Cursos { get; set; }
    }
}
