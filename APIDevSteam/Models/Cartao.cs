namespace APIDevSteam.Models
{
    public class Cartao
    {
        public Guid CartaoId { get; set; }
        public string NomeTitular { get; set; }
        public int numeroCartao { get; set; }
        public int codSeguranca { get; set; }
        public DateOnly dataValidade { get; set; }

        public ICollection<UsuarioCartao> UsuarioCartoes { get; set; } = new List<UsuarioCartao>();

    }
}
