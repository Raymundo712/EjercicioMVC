using CajaVirtualCobrosAPI.Entidades;
using Microsoft.EntityFrameworkCore;

namespace CajaVirtualCobrosAPI
{
    public class ApplicationDbContext :DbContext
    {
        public ApplicationDbContext(DbContextOptions options):base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<EstadoCuenta> EstadosCuenta { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<ConceptoCobro> Conceptos { get; set; }
        public DbSet<Movimiento> Movimientos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>().HasOne(u => u.Rol).WithMany().HasForeignKey(u => u.Rol_Id);

            modelBuilder.Entity<Cliente>().HasOne(u => u.Usuario).WithOne(u => u.Cliente).HasForeignKey<Cliente>(c => c.Usuario_Id);

            modelBuilder.Entity<EstadoCuenta>().HasOne(ec => ec.Cliente).WithOne(m => m.EstadoCuenta).HasForeignKey<EstadoCuenta>(ec => ec.Cliente_Id);

            modelBuilder.Entity<Movimiento>().HasOne(m => m.EstadoCuenta).WithMany().HasForeignKey(m => m.EstadoCuenta_Id);

            modelBuilder.Entity<Movimiento>().HasOne(m => m.ConceptoCobro).WithMany().HasForeignKey(m => m.ConceptoCobro_Id);

            base.OnModelCreating(modelBuilder);
        }


    }
}
