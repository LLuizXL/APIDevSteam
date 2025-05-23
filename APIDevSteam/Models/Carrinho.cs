﻿namespace APIDevSteam.Models
{
    public class Carrinho
    {
        public Guid CarrinhoId { get; set; }
        public Guid? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataFinalizacao { get; set; }
        public bool? Finalizado { get; set; }

        public decimal ValorTotal { get; set; }

        // Propriedade de navegação para os itens do carrinho
        public ICollection<ItemCarrinho> ItensCarrinhos { get; set; } = new List<ItemCarrinho>();
    }
}
