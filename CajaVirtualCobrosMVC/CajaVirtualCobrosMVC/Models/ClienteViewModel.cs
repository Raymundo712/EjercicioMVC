using CajaVirtualCobrosMVC.Entidades;

namespace CajaVirtualCobrosMVC.Models
{
    public class ClienteViewModel
    {
        public Cliente? Cliente { get; set; }
        public Usuario? Usuario { get; set;}
        public ICollection<Rol>? Roles { get; set; }
        public int? Rol_Id { get; set; }
    }
}
