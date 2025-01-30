using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CajaVirtualCobrosAPI.Entidades
{
    public class EstadoCuenta
    {
        public int Id { get; set; }

        [Column(TypeName="Money")]
        public decimal Saldo { get; set; }

        public int Cliente_Id { get; set; }

        public Cliente? Cliente { get; set; }

        public ICollection<Movimiento>? Movimientos { get; set; }

    }
}
