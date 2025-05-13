namespace APIDevSteam.Models
{
    public class UsuarioCartao
    {
        public Guid UsuarioCartaoId { get; set; }
        public Guid UsuarioId { get; set; }
        public Guid CartaoId { get; set; }
        public Cartao? Cartao { get; set; }
        public Usuario? Usuario { get; set; }


    }
}
