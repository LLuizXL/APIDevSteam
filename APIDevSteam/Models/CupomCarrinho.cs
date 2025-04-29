namespace APIDevSteam.Models
{
    public class CupomCarrinho
    {
        public Guid CupomCarrinhoId { get; set; }
        public Guid CarrinhoId { get; set; }
        public Guid CupomId { get; set; }
        public Cupom? Cupom { get; set; }
        public Carrinho? Carrinho { get; set; }
        public DateTime? DataAplicacao { get; set; }

    }
}
