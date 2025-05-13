namespace APIDevSteam.Models
{
    public class UsuarioCartao
    {
        public Guid usuarioCartaoId { get; set; }
        public Guid UsuarioId { get; set; }
        public Guid CartaoId { get; set; }
        public Usuario? Usuario { get; set; }
        public Cartao? Cartao { get; set; }


    }
}
