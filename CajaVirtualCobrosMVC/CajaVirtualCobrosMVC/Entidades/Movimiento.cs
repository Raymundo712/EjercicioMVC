using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace CajaVirtualCobrosMVC.Entidades
{
    public class Movimiento
    {
        public int Id { get; set; }

        public int EstadoCuenta_Id { get; set; }

        public DateTime FechaMovimiento { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? ConceptoCobro_Id { get; set; }

        [Column(TypeName = "Money")]
        public decimal Abono { get; set; }

        public EstadoCuenta? EstadoCuenta { get; set; }

        public ConceptoCobro? ConceptoCobro { get; set; }
    }
}
