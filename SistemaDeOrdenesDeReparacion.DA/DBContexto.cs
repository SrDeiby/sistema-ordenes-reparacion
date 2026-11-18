using Microsoft.EntityFrameworkCore;

namespace SistemaDeOrdenesDeReparacion.DA
{
    public class DBContexto : DbContext
    {
        public DBContexto(DbContextOptions<DBContexto> options) : base(options)
        {
        }

      
        public DbSet<SistemaDeOrdenesDeReparacion.Model.Cliente> Clientes { get; set; }
        public DbSet<SistemaDeOrdenesDeReparacion.Model.Mecanico> Mecanicos { get; set; }
        public DbSet<SistemaDeOrdenesDeReparacion.Model.Usuario> Usuarios { get; set; }

        public DbSet<SistemaDeOrdenesDeReparacion.Model.OrdenesDeTrabajos> OrdenesDeTrabajos { get; set; }
        public DbSet<SistemaDeOrdenesDeReparacion.Model.InventarioDeRepuestos> InventarioDeRepuestos { get; set; }
        public DbSet<SistemaDeOrdenesDeReparacion.Model.InventarioDeServicios> InventarioDeServicios { get; set; }

        public DbSet<SistemaDeOrdenesDeReparacion.Model.OrdenesDeTrabajoInventarioDeServicios> OrdenesDeTrabajoInventarioDeServicios { get; set; }

        public DbSet<SistemaDeOrdenesDeReparacion.Model.OrdenesDeTrabajoInventarioDeRepuestos> OrdenesDeTrabajoInventarioDeRepuestos { get; set; }

    }
}
