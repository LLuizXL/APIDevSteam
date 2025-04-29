using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIDevSteam.Data;
using APIDevSteam.Models;
using APIDevSteam.Migrations;

namespace APIDevSteam.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CuponsCarrinhoController : ControllerBase
    {
        private readonly APIContext _context;

        public CuponsCarrinhoController(APIContext context)
        {
            _context = context;
        }

        // GET: api/CuponsCarrinho
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CupomCarrinho>>> GetCuponsCarrinho()
        {
            return await _context.CuponsCarrinho.ToListAsync();
        }

        // GET: api/CuponsCarrinho/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CupomCarrinho>> GetCupomCarrinho(Guid id)
        {
            var cupomCarrinho = await _context.CuponsCarrinho.FindAsync(id);

            if (cupomCarrinho == null)
            {
                return NotFound();
            }

            return cupomCarrinho;
        }

        // PUT: api/CuponsCarrinho/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCupomCarrinho(Guid id, CupomCarrinho cupomCarrinho)
        {
            if (id != cupomCarrinho.CupomCarrinhoId)
            {
                return BadRequest();
            }

            _context.Entry(cupomCarrinho).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CupomCarrinhoExists(id))
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

        // POST: api/CuponsCarrinho
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CupomCarrinho>> PostCupomCarrinho(CupomCarrinho cupomCarrinho)
        {
            _context.CuponsCarrinho.Add(cupomCarrinho);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCupomCarrinho", new { id = cupomCarrinho.CupomCarrinhoId }, cupomCarrinho);
        }

        // DELETE: api/CuponsCarrinho/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCupomCarrinho(Guid id)
        {
            var cupomCarrinho = await _context.CuponsCarrinho.FindAsync(id);
            if (cupomCarrinho == null)
            {
                return NotFound();
            }

            _context.CuponsCarrinho.Remove(cupomCarrinho);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CupomCarrinhoExists(Guid id)
        {
            return _context.CuponsCarrinho.Any(e => e.CupomCarrinhoId == id);
        }

        // Adicionar um cupom de desconto no valor total da venda
        [HttpPost("AdicionarCupom")]
        public async Task<IActionResult> AdicionarCupom(Guid idCarrinho, Guid idCupom)
        {
            //Verifica se o carrinho existe
            var carrinho = await _context.Carrinhos.FindAsync(idCarrinho);
            var cupom = await _context.Cupons.FindAsync(idCupom);

            if (carrinho == null) return NotFound("Carrinho não encontrado.");
            if (cupom == null) return NotFound("Cupom não encontrado.");

            if (carrinho.Finalizado == true)
            {
                return BadRequest(" A compra já foi finalizada.");
            }

            var cupomCarrinho = await _context.CuponsCarrinho
               .FirstOrDefaultAsync(cc => cc.CarrinhoId == idCarrinho && cc.CupomId == idCupom);
            if (cupomCarrinho != null)
            {
                return BadRequest("Cupom já aplicado no carrinho!");
            }

            if (cupom.Ativo == false)
            {
                return BadRequest("cupom não está ativo.");
            }

            if (cupom.LimiteUso <= 0)
            {
                return BadRequest("cupom já atingiu seu limite.");
            }


            if (cupom.DataValidade != null)
            {
                if (cupom.DataValidade < DateTime.Now)
                {
                    cupom.Ativo = false;
                    return BadRequest("Este cupom já expirou.");
                }
            }

            //Validar o cupom para que não seja aplicado várias vezes no mesmo carrinho
            //idCarrinho.ValorTotal = carrinho.ValorTotal - (carrinho.ValorTotal * (cupom.Desconto / 100));

            //cupom.LimiteUso -= 1;


            // Aplicando o cupom no carrinho 
            cupomCarrinho = new CupomCarrinho
            {
                CupomCarrinhoId = Guid.NewGuid(),
                CarrinhoId = idCarrinho,
                CupomId = idCupom,
                DataAplicacao = DateTime.Now
            };

            _context.CuponsCarrinho.Add(cupomCarrinho);

            // Atualizando o limite de uso do cupom, caso seja diferente de nulo
            // Se atingir o limite de uso, o cupom será desativado
            if (cupom.LimiteUso != null)
            {
                cupom.LimiteUso--;
                if (cupom.LimiteUso == 0)
                {
                    cupom.Ativo = false;
                }
            }
            _context.Entry(cupom).State = EntityState.Modified;

            await _context.SaveChangesAsync();


            return Ok(cupomCarrinho);
        }
    }
}
