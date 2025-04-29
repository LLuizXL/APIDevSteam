namespace APIDevSteam.Models
{
    public class JogoCategoria
    {
        public Guid JogoCategoriaId { get; set; }
        public Guid GameId { get; set; }
        public Game? Game { get; set; }
        public Guid CategoriaId { get; set; }
        public Categoria? Categoria { get; set; }

    }
}
