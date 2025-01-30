using System.ComponentModel.DataAnnotations;

namespace CajaVirtualCobrosMVC.Entidades
{
    public class Cliente
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string? Nombre { get; set; }

        [StringLength(15)]
        public string? NumeroCliente { get; set; }

        public EstadoCuenta? EstadoCuenta { get; set; }

        public int? Usuario_Id { get; set; }

        public Usuario? Usuario { get; set; }
    }
}
