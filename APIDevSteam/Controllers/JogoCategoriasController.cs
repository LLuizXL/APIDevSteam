using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIDevSteam.Data;
using APIDevSteam.Models;
using Microsoft.AspNetCore.Authorization;

namespace APIDevSteam.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JogoCategoriasController : ControllerBase
    {
        private readonly APIContext _context;

        public JogoCategoriasController(APIContext context)
        {
            _context = context;
        }

        // GET: api/JogoCategorias
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JogoCategoria>>> GetJogosCategoria()
        {
            return await _context.JogosCategoria.ToListAsync();
        }

        // GET: api/JogoCategorias/5
        [HttpGet("{id}")]
        public async Task<ActionResult<JogoCategoria>> GetJogoCategoria(Guid id)
        {
            var jogoCategoria = await _context.JogosCategoria.FindAsync(id);

            if (jogoCategoria == null)
            {
                return NotFound();
            }

            return jogoCategoria;
        }

        // PUT: api/JogoCategorias/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutJogoCategoria(Guid id, JogoCategoria jogoCategoria)
        {
            if (id != jogoCategoria.JogoCategoriaId)
            {
                return BadRequest();
            }

            _context.Entry(jogoCategoria).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JogoCategoriaExists(id))
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

        // POST: api/JogoCategorias
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<JogoCategoria>> PostJogoCategoria(JogoCategoria jogoCategoria)
        {
            _context.JogosCategoria.Add(jogoCategoria);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetJogoCategoria", new { id = jogoCategoria.JogoCategoriaId }, jogoCategoria);
        }

        // DELETE: api/JogoCategorias/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJogoCategoria(Guid id)
        {
            var jogoCategoria = await _context.JogosCategoria.FindAsync(id);
            if (jogoCategoria == null)
            {
                return NotFound();
            }

            _context.JogosCategoria.Remove(jogoCategoria);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool JogoCategoriaExists(Guid id)
        {
            return _context.JogosCategoria.Any(e => e.JogoCategoriaId == id);
        }
    }
}
