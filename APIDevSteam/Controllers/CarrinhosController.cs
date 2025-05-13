using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIDevSteam.Data;
using APIDevSteam.Models;
using System.Security.Claims;
using APIDevSteam.Migrations;

namespace APIDevSteam.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarrinhosController : ControllerBase
    {
        private readonly APIContext _context;

        public CarrinhosController(APIContext context)
        {
            _context = context;
        }

        // GET: api/Carrinhos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Carrinho>>> GetCarrinhos()
        {
            return await _context.Carrinhos.ToListAsync();
        }



        // GET: api/Carrinhos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Carrinho>> GetCarrinho(Guid id)
        {
            var carrinho = await _context.Carrinhos.FindAsync(id);

            if (carrinho == null)
            {
                return NotFound();
            }

            return carrinho;
        }

        // PUT: api/Carrinhos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCarrinho(Guid id, Carrinho carrinho)
        {
            if (id != carrinho.CarrinhoId)
            {
                return BadRequest();
            }

            _context.Entry(carrinho).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CarrinhoExists(id))
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

        // POST: api/Carrinhos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Carrinho>> PostCarrinho(Carrinho carrinho)
        {


            _context.Carrinhos.Add(carrinho);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCarrinho", new { id = carrinho.CarrinhoId }, carrinho);
        }

        // DELETE: api/Carrinhos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCarrinho(Guid id)
        {
            var carrinho = await _context.Carrinhos.FindAsync(id);
            if (carrinho == null)
            {
                return NotFound();
            }

            _context.Carrinhos.Remove(carrinho);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CarrinhoExists(Guid id)
        {
            return _context.Carrinhos.Any(e => e.CarrinhoId == id);
        }
        [HttpPost("FinalizarCompra/{id}")]
        public async Task<IActionResult> FinalizarCompra(Guid id)
        {
            // Verifica se o carrinho existe  
            var carrinho = await _context.Carrinhos
                .Include(c => c.Usuario)
                .Include(c => c.ItensCarrinhos)
                .ThenInclude(ic => ic.Game)
                .FirstOrDefaultAsync(c => c.CarrinhoId == id);
            if (carrinho == null)
            {
                return NotFound();
            }

            // Pegar o Id do IdentityUser logado a partir do token  
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (usuarioId == null)
            {
                return Unauthorized("Usuário não autenticado.");
            }

            // Verifica se o carrinho já foi finalizado  
            if (carrinho.Finalizado == true)
            {
                return BadRequest("Carrinho já foi finalizado.");
            }

            // Verifica se o carrinho está vazio  
            var itensCarrinho = await _context.ItensCarrinhos.Where(i => i.CarrinhoId == id).ToListAsync();
            if (itensCarrinho.Count == 0 || !carrinho.ItensCarrinhos.Any())
            {
                return BadRequest("Carrinho vazio.");
            }

            // Calcula o valor total do carrinho  
            decimal valorTotal = 0;
            foreach (var item in itensCarrinho)
            {
                var jogo = await _context.Jogos.FindAsync(item.GameId);
                if (jogo != null)
                {
                    valorTotal += jogo.Preco;
                }
            }

            // Adiciona os jogos comprados à lista de jogos do usuário  
            foreach (var item in carrinho.ItensCarrinhos)
            {
                // Verifica se o jogo já foi comprado pelo usuário  
                var jogoJaComprado = await _context.UsuariosJogos
                    .AnyAsync(uj => uj.UsuarioId == carrinho.UsuarioId.ToString() && uj.GameId == item.GameId);

                if (!jogoJaComprado)
                {
                    var usuarioJogo = new UsuarioJogo
                    {
                        UsuarioId = carrinho.UsuarioId.ToString(),
                        GameId = item.GameId.Value,
                        DataCompra = DateTime.UtcNow
                    };

                    _context.UsuariosJogos.Add(usuarioJogo);
                }
            }

            // Atualiza o carrinho  
            carrinho.Finalizado = true;
            carrinho.DataFinalizacao = DateTime.Now;
            carrinho.UsuarioId = Guid.Parse(usuarioId);
            carrinho.ValorTotal = valorTotal;

            _context.Entry(carrinho).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok("Compra finalizada com sucesso");
        }
    }
}
