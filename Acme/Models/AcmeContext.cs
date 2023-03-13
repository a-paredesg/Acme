using System.Data.Entity;

namespace Acme.Models
{
    public class AcmeContext : DbContext
    {
        public AcmeContext() : base("AcmeContext")
        {
        }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Encuesta> Encuestas { get; set; }
        public DbSet<Campo> Campos { get; set; }
    }
}