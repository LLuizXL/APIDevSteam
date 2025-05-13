using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIDevSteam.Data;
using APIDevSteam.Models;

namespace APIDevSteam.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItensCarrinhoController : ControllerBase
    {
        private readonly APIContext _context;

        public ItensCarrinhoController(APIContext context)
        {
            _context = context;
        }

        // GET: api/ItensCarrinho
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemCarrinho>>> GetItensCarrinhos()
        {
            return await _context.ItensCarrinhos.ToListAsync();
        }

        // GET: api/ItensCarrinho/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemCarrinho>> GetItemCarrinho(Guid id)
        {
            var itemCarrinho = await _context.ItensCarrinhos.FindAsync(id);

            if (itemCarrinho == null)
            {
                return NotFound();
            }

            return itemCarrinho;
        }

        // PUT: api/ItensCarrinho/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutItemCarrinho(Guid id, ItemCarrinho itemCarrinho)
        {
            if (id != itemCarrinho.ItemCarrinhoId)
            {
                return BadRequest();
            }

            _context.Entry(itemCarrinho).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemCarrinhoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ItensCarrinho
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ItemCarrinho>> PostItemCarrinho(ItemCarrinho itemCarrinho)
        {
            // Verifica se o carrinho existe
            var carrinho = await _context.Carrinhos.FindAsync(itemCarrinho.CarrinhoId);
            if (carrinho == null)
            {
                return NotFound("Carrinho não encontrado.");
            }

            // Verifica se o jogo existe
            var jogo = await _context.Jogos.FindAsync(itemCarrinho.GameId);
            if (jogo == null)
            {
                return NotFound("Jogo não encontrado.");
            }

            // Verifica se o jogo já está no carrinho
            var itemExistente = await _context.ItensCarrinhos
                .FirstOrDefaultAsync(ic => ic.CarrinhoId == itemCarrinho.CarrinhoId && ic.GameId == itemCarrinho.GameId);

            if (itemExistente != null)
            {
                // Atualiza a quantidade e o valor do item existente
                itemExistente.Quantidade += itemCarrinho.Quantidade;

                // Calcula o valor com desconto ou sem desconto
                if (jogo.Desconto > 0)
                {
                    itemExistente.ValorTotal = itemExistente.Quantidade * (jogo.Preco - (jogo.Preco * (jogo.Desconto / 100)));
                }
                else
                {
                    itemExistente.ValorTotal = itemExistente.Quantidade * jogo.Preco;
                }

                // Atualiza o valor total do carrinho
                carrinho.ValorTotal += itemCarrinho.Quantidade * (jogo.Desconto > 0
                    ? (jogo.Preco - (jogo.Preco * (jogo.Desconto / 100)))
                    : jogo.Preco);
            }
            else
            {
                // Calcula o valor com desconto ou sem desconto
                if (jogo.Desconto > 0)
                {
                    itemCarrinho.ValorTotal = itemCarrinho.Quantidade * (jogo.Preco - (jogo.Preco * (jogo.Desconto / 100)));
                }
                else
                {
                    itemCarrinho.ValorTotal = itemCarrinho.Quantidade * jogo.Preco;
                }

                // Adiciona o valor total do novo item ao carrinho
                carrinho.ValorTotal += itemCarrinho.ValorTotal;

                // Adiciona o novo item ao carrinho
                _context.ItensCarrinhos.Add(itemCarrinho);
            }

            // Salva as alterações no banco de dados
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetItemCarrinho", new { id = itemCarrinho.ItemCarrinhoId }, itemCarrinho);
        }

        // DELETE: api/ItensCarrinho/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItemCarrinho(ItemCarrinho itemCarrinho)
        {




            var carrinho = await _context.Carrinhos.FindAsync(itemCarrinho.CarrinhoId);
            if (carrinho == null)
            {
                return NotFound("Carrinho não encontrado.");
            }

            // Remove o valor total do carrinho
            carrinho.ValorTotal -= itemCarrinho.ValorTotal;


            //Verifica se o valor total do carrinho é menor que 0

            if (carrinho.ValorTotal < 0)
            {
                carrinho.ValorTotal = 0;
            }

            _context.ItensCarrinhos.Remove(itemCarrinho);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ItemCarrinhoExists(Guid id)
        {
            return _context.ItensCarrinhos.Any(e => e.ItemCarrinhoId == id);
        }
        // Aumentar a quantidade de um jogo, recebendo o id do jogo e verificar se ele existe no carrinho
        [HttpPut("AumentarQuantidade/{id}")]
        public async Task<ActionResult<ItemCarrinho>> AumentarQuantidade(Guid id)
        {
            var itemCarrinho = await _context.ItensCarrinhos.FindAsync(id);
            if (itemCarrinho == null)
            {
                return NotFound("Item não encontrado.");
            }
            // Verifica se o jogo existe
            var jogo = _context.Jogos.Find(itemCarrinho.GameId);
            if (jogo == null)
            {
                return NotFound("Jogo não encontrado.");
            }
            // Aumenta a quantidade
            itemCarrinho.Quantidade++;
            itemCarrinho.ValorTotal = itemCarrinho.Quantidade * jogo.Preco;

            // Alterar o valor total do carrinho
            var carrinho = await _context.Carrinhos.FindAsync(itemCarrinho.CarrinhoId);
            if (carrinho == null)
            {
                return NotFound("Carrinho não encontrado.");
            }
            carrinho.ValorTotal += jogo.Preco;
            _context.Entry(carrinho).State = EntityState.Modified;

            _context.SaveChanges();
            return Ok(itemCarrinho);
        }

        // Diminuir a quantidade de um jogo, recebendo o id do jogo e verificar se ele existe no carrinho
        [HttpPut("DiminuirQuantidade/{id}")]
        public async Task<ActionResult<ItemCarrinho>> DiminuirQuantidade(Guid id)
        {
            var itemCarrinho = await _context.ItensCarrinhos.FindAsync(id);
            if (itemCarrinho == null)
            {
                return NotFound("Item não encontrado.");
            }
            // Verifica se o jogo existe
            var jogo = await _context.Jogos.FindAsync(itemCarrinho.GameId);
            if (jogo == null)
            {
                return NotFound("Jogo não encontrado.");
            }
            // Diminui a quantidade
            if (itemCarrinho.Quantidade > 1)
            {
                itemCarrinho.Quantidade--;
                itemCarrinho.ValorTotal = itemCarrinho.Quantidade * jogo.Preco;

                // Alterar o valor total do carrinho
                var carrinho = await _context.Carrinhos.FindAsync(itemCarrinho.CarrinhoId);
                if (carrinho == null)
                {
                    return NotFound("Carrinho não encontrado.");
                }
                carrinho.ValorTotal -= jogo.Preco;
                _context.Entry(carrinho).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                return Ok(itemCarrinho);
            }
            else
            {
                return BadRequest("A quantidade não pode ser menor que 1.");
            }
        }
    }
}
