namespace APIDevSteam.Models
{
    public class UsuarioJogo
    {
        public Guid UsuarioJogoId { get; set; }
        public string UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        public Guid GameId { get; set; }
        public Game Game { get; set; }
        public DateTime DataCompra { get; set; }
    }
}
