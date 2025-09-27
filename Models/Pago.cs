using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace ProgramaYA.Models
{
    public class Pago
    {
        public int? Id { get; set; }
        public string? Titular { get; set; }
        public string? TipoTarjeta { get; set; }
        public string? NumeroTarjeta { get; set; }
        public int? Total { get; set; }
        public string? FechaVencimiento { get; set; }
        public string? CVV { get; set; }
        [JsonIgnore]
        public Suscripcion Suscripcion { get; set; }
    }
}
