using System.ComponentModel.DataAnnotations;

namespace CajaVirtualCobrosMVC.Entidades
{
    public class Rol
    {
        public int Id { get; set; }

        [StringLength(30)]
        public string? Nombre { get; set; }

        public Usuario? Usuario { get; set; }
    }
}
