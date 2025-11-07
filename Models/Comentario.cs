using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using ProgramaYA.Models;

namespace apptienda.Models
{
    [Table("t_comentario")]
    public class Comentario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }        
        [ForeignKey("Curso")]
        [Column("curso_id")]
        public int? CursoSeleccionado { get; set; }
        [JsonIgnore]
        public Curso? Curso { get; set; }
        [NotNull]
        public string? Email { get; set; }
        [NotNull]
        public string? Mensaje { get; set; }

        public string? Etiqueta { get; set; }

        public float Puntuacion { get; set; }

    }
}