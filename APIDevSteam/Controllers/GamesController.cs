using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIDevSteam.Data;
using APIDevSteam.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;

namespace APIDevSteam.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly APIContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly RoleManager<IdentityRole> _roleManager;

        public GamesController(APIContext context, IWebHostEnvironment webHostEnvironment, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _roleManager = roleManager;
        }

        // GET: api/Games
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Game>>> GetJogos()
        {
            return await _context.Jogos.ToListAsync();
        }

        // GET: api/Games/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Game>> GetGame(Guid id)
        {
            var game = await _context.Jogos.FindAsync(id);

            if (game == null)
            {
                return NotFound();
            }

            return game;
        }

        // PUT: api/Games/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGame(Guid id, Game game)
        {
            if (id != game.GameId)
            {
                return BadRequest();
            }

            _context.Entry(game).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameExists(id))
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

        // POST: api/Games
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Game>> PostGame(Game game)
        {
            _context.Jogos.Add(game);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGame", new { id = game.GameId }, game);
        }

        // DELETE: api/Games/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(Guid id)
        {
            var game = await _context.Jogos.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }

            _context.Jogos.Remove(game);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GameExists(Guid id)
        {
            return _context.Jogos.Any(e => e.GameId == id);
        }


        // [HttpPOST] : Upload da Foto de Game
        [HttpPost("UploadGamePicture")]
        public async Task<IActionResult> UploadProfilePicture(IFormFile file, Guid JogoId)
        {
            // Verifica se o arquivo é nulo ou vazio
            if (file == null || file.Length == 0)
                return BadRequest("Arquivo não pode ser nulo ou vazio.");

            // Verifica se o jogo existe
            var jogo = await _context.Jogos.FindAsync(JogoId);
            if (jogo == null)
                return NotFound("Usuário não encontrado.");

            // Verifica se o arquivo é uma imagem
            if (!file.ContentType.StartsWith("image/"))
                return BadRequest("O arquivo deve ser uma imagem.");

            // Define o caminho para salvar a imagem na pasta Resources/Profile
            var gameFolder = Path.Combine(_webHostEnvironment.ContentRootPath, "Resources", "Games");
            if (!Directory.Exists(gameFolder))
                Directory.CreateDirectory(gameFolder);

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (Array.IndexOf(allowedExtensions, fileExtension) < 0)
                return BadRequest("Formato de arquivo não suportado. Use .jpg, .jpeg, .png ou .gif.");

            var fileName = $"{jogo.GameId}{fileExtension}";
            var filePath = Path.Combine(gameFolder, fileName);


            // Verifica se o arquivo já existe e o remove
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // Salva o arquivo no disco
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Retorna o caminho relativo da imagem
            var relativePath = Path.Combine("Resources", "Games", fileName).Replace("\\", "/");

            // Atualiza o Campo Banner do Jogo
            jogo.imageBanner = fileName;
            _context.Entry(jogo).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Retorna o caminho da imagem
            return Ok(new { FilePath = relativePath });
        }


        // [HttpGET] : Buscar a imagem de jogo e retornar como Base64
        [HttpGet("GetGamePicture")]
        public async Task<IActionResult> GetProfilePicture(Guid jogoId)
        {
            // Verifica se o jogo existe
            var jogo = await _context.Jogos.FindAsync(jogoId);
            if (jogo == null)
                return NotFound("Usuário não encontrado.");

            // Caminho da imagem na pasta Resources/Profile
            var gameFolder = Path.Combine(_webHostEnvironment.ContentRootPath, "Resources", "Games");
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

            // Procura a imagem do usuário com base no ID
            string? userImagePath = null;
            foreach (var extension in allowedExtensions)
            {
                var potentialPath = Path.Combine(gameFolder, $"{jogo.GameId}{extension}");
                if (System.IO.File.Exists(potentialPath))
                {
                    userImagePath = potentialPath;
                    break;
                }
            }
            // Se a imagem não for encontrada
            if (userImagePath == null)
                return NotFound("Imagem de perfil não encontrada.");

            // Lê o arquivo como um array de bytes
            var imageBytes = await System.IO.File.ReadAllBytesAsync(userImagePath);

            // Converte os bytes para Base64
            var base64Image = Convert.ToBase64String(imageBytes);

            // Retorna a imagem em Base64
            return Ok(new { Base64Image = $"data:image/{Path.GetExtension(userImagePath).TrimStart('.')};base64,{base64Image}" });
        }

        [HttpPut("RemoverDesconto")]
        public async Task<IActionResult> RemoverDesconto(Guid jogoId)
        {

            //Verifica se o jogo existe
            var jogo = await _context.Jogos.FindAsync(jogoId);
            if (jogo == null) return NotFound("Jogo não encontrado.");

            //Remove o desconto
            jogo.Desconto = 0;
            jogo.Preco = (decimal)jogo.PrecoOriginal;

            //Atualiza o jogo no Banco de Dados
            _context.Entry(jogo).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(jogo);
        }


        [HttpPut("AplicarDesconto")]
        public async Task<IActionResult> AplicarDesconto(Guid jogoId, int desconto)
        {

            //Verifica se o jogo existe
            var jogo = await _context.Jogos.FindAsync(jogoId);
            if (jogo == null) return NotFound("Jogo não encontrado.");

            //Verifica o valor do desconto
            if (desconto < 0 || desconto > 100)
                return BadRequest("Desconto deve ser entre 0 e 100.");

            //Aplica o desconto
            jogo.Desconto = 0;
            jogo.Preco = (decimal)(jogo.PrecoOriginal - (jogo.PrecoOriginal * (desconto / 100)));


            //Atualiza o jogo no Banco de Dados
            _context.Entry(jogo).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(jogo);
        }

    }

}
