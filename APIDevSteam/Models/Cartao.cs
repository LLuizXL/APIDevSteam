namespace APIDevSteam.Models
{
    public class Cartao
    {
        public Guid CartaoId { get; set; }
        public string NomeTitular { get; set; }
        public string numeroCartao { get; set; }
        public int codSeguranca { get; set; }
        public DateOnly dataValidade { get; set; }

        public Guid? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

    }
}
