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
        public DbSet<Game> Jogos { get; set; } // DbSet para a Classe Jogos
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<JogoCategoria> JogosCategoria { get; set; }
        public DbSet<JogoMidia> JogosMidia { get; set; }
        public DbSet<Carrinho> Carrinhos { get; set; } // DbSet para a Classe Comentario
        public DbSet<ItemCarrinho> ItensCarrinhos { get; set; } // DbSet para a Classe Comentario
        public DbSet<Cupom> Cupons { get; set; }
        public DbSet<CupomCarrinho> CuponsCarrinho { get; set; } // DbSet para a Classe Comentario
        public DbSet<Cartao> Cartoes { get; set; } // DbSet para a Classe Comentario
        public DbSet<UsuarioCartao> UsuarioCartoes { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configurações adicionais do modelo, se necessário

            modelBuilder.Entity<Usuario>().ToTable("Usuarios"); // Mapeia a entidade Usuario para a tabela "Usuarios"
            modelBuilder.Entity<Game>().ToTable("Jogos");
            modelBuilder.Entity<Categoria>().ToTable("Categorias");
            modelBuilder.Entity<JogoCategoria>().ToTable("JogosCategorias");
            modelBuilder.Entity<JogoMidia>().ToTable("JogosMidia");
            modelBuilder.Entity<Carrinho>().ToTable("Carrinhos");
            modelBuilder.Entity<ItemCarrinho>().ToTable("ItensCarrinhos");
            modelBuilder.Entity<Cupom>().ToTable("Cupons");
            modelBuilder.Entity<CupomCarrinho>().ToTable("CuponsCarrinho");
            modelBuilder.Entity<UsuarioCartao>().ToTable("UsuarioCartoes");
            modelBuilder.Entity<Cartao>().ToTable("Cartoes");

        }


    }
}
