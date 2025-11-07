using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using apptienda.Models;
using Microsoft.AspNetCore.Identity;

namespace ProgramaYA.Models
{
    public class Curso
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int? Id { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public string? Nivel { get; set; }
        public int? Precio { get; set; }
        public string? Imagen { get; set; }
        public List<Capitulo>? Capitulos { get; set; }
        public List<ApplicationUser>? Usuarios { get; set; }
    }
    public class ComentarioCursos
    {
        public Comentario? Comentario { get; set; }
        public List<CursosList>? Cursos { get; set; }       
    }
    public class CursosList
    {
        public int? Id { get; set; }
        public string? Nombre { get; set; }       
    }
}
