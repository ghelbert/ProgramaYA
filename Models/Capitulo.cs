using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace ProgramaYA.Models
{
    public class Capitulo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int? Id { get; set; }
        public string? Titulo { get; set; }
        public string? Video { get; set; }
        public int? CursoId { get; set; }
        [JsonIgnore]
        public Curso? Curso { get; set; }
    }
}
