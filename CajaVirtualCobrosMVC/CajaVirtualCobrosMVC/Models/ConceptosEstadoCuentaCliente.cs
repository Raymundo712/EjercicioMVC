using CajaVirtualCobrosMVC.Entidades;

namespace CajaVirtualCobrosMVC.Models
{
    public class ConceptosEstadoCuentaCliente
    {
        public Cliente Cliente { get; set; }

        public EstadoCuenta EstadoCuenta { get; set; }

        public Movimiento Movimiento { get; set; }

        public ICollection<Movimiento> Movimientos { get; set; }

        public ICollection<ConceptoCobro> Conceptos { get; set; }

        public int ConceptoCobroId { get; set; }
    }
}
