using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace ProgramaYA.Models
{
    public class Suscripcion
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int? Id { get; set; }
        public string? ApplicationUserId { get; set; }
        public int? CursoId { get; set; }
        [JsonIgnore]
        public ApplicationUser? Usuario { get; set; }
        [JsonIgnore]
        public Curso? Curso { get; set; }
        public string? Meses { get; set; }
        public DateOnly? FechaInicio { get; set; }
        public DateOnly? FechaTermino { get; set; }
    }
}
