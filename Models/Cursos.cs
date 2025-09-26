using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ProgramaYA.Models
{
    public class Curso
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int? Id { get; set; }
        public string? Nombre { get; set; }
        public string? Nivel { get; set; }
        public string? Imagen { get; set; }
    }
}
