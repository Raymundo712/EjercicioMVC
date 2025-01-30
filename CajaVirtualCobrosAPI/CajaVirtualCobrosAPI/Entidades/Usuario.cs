using System.ComponentModel.DataAnnotations;

namespace CajaVirtualCobrosAPI.Entidades
{
    public class Usuario
    {
        public int Id { get; set; }
            
        [StringLength(15)]
        public string? UsuarioNombre { get; set; }
               
        [StringLength(15)]
        public string? Contrasena { get; set; }

        public int Rol_Id { get; set; }

        public Rol? Rol { get; set; }

        public Cliente? Cliente { get; set; }

    }
}
