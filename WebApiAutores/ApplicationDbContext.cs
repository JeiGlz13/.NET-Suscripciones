using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Entidades;

namespace WebApiAutores
{
    public class ApplicationDbContext : IdentityDbContext<Usuario>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AutorLibro>()
                        .HasKey(al => new {al.AutorId, al.LibroId});

            modelBuilder.Entity<Factura>()
                        .Property(x => x.Monto).HasColumnType("decimal(18, 2)");
        }

        public DbSet<Autor>Autores { get; set; }
        public DbSet<Libro>Libros { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }
        public DbSet<AutorLibro> AutorLibro { get; set; }
        public DbSet<LlaveAPI> LlaveAPIs { get; set; }
        public DbSet<Peticion> Peticiones { get; set; }
        public DbSet<RestriccionesDominio> RestriccionesDominios { get; set; }
        public DbSet<RestriccionIP> RestriccionIPs { get; set; }
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<FacturaEmitida> FacturasEmitidas { get; set; }
    }
}
