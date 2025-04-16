using APIDevSteam.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace APIDevSteam.Data
{
    public class APIContext : IdentityDbContext<Usuario>
    {
        public APIContext(DbContextOptions<APIContext> options) : base(options)
        {

        }

        public DbSet<Usuario> Usuarios { get; set; } // DbSet para a entidade Usuario


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configurações adicionais do modelo, se necessário

            modelBuilder.Entity<Usuario>().ToTable("Usuarios"); // Mapeia a entidade Usuario para a tabela "Usuarios"
        }


    }
}
