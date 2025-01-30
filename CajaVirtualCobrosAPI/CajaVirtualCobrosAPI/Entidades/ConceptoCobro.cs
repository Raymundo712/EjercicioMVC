using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CajaVirtualCobrosAPI.Entidades
{
    public class ConceptoCobro
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? Nombre { get; set; }
        
        [Required]
        [Column(TypeName = "Money")]
        public decimal Valor { get; set; }

    }
}
