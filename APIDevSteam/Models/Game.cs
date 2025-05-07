namespace APIDevSteam.Models
{
    public class Game
    {
        public Guid GameId { get; set; }
        public string Titulo { get; set; }
        public decimal Preco { get; set; }
        public int Desconto { get; set; }
        public string imageBanner { get; set; }
        public string Descricao { get; set; }

        public decimal? PrecoOriginal { get; set; }
        public bool? Lancamento { get; set; } = false;

        // Lista de usuários que compraram o jogo
        public ICollection<UsuarioJogo> UsuariosCompradores { get; set; } = new List<UsuarioJogo>();

    }
}
