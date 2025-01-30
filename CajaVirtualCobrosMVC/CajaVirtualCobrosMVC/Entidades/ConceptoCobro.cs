using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CajaVirtualCobrosMVC.Entidades
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
