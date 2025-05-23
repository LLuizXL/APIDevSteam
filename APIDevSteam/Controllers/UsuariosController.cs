﻿using APIDevSteam.Data;
using APIDevSteam.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Validations;
using System.Threading.RateLimiting;
using static System.Net.Mime.MediaTypeNames;

namespace APIDevSteam.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _webHostEnvironment; // Informações do Host do servidor web
        private readonly APIContext _context;

        //Método Construtor com as injeções de dependência

        public UsuariosController(UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager, IWebHostEnvironment webHostEnvironment, APIContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _webHostEnvironment = webHostEnvironment;
            _context = context;

        }

        [Authorize]
        [HttpPost("CreateRole")] // Post: Criar uma Role
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
                return BadRequest("O nome do cargo não pode estar vazio.");

            if (await _roleManager.RoleExistsAsync(roleName))
                return BadRequest("Este cargo já existe ou foi criado.");

            var role = new IdentityRole(roleName);
            var result = await _roleManager.CreateAsync(role); //Aguarda a confirmação da criação da role

            if (result.Succeeded) //Caso o resultado tenha tido sucesso
                return Ok($"Cargo '{roleName}' criado com sucesso.");

            else
                return BadRequest(result.Errors);

        }

        [Authorize]
        [HttpPost("AddRoleToUser")] // Post: Adicionar um Usuário a uma Role
        public async Task<IActionResult> AddRoleToUser(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId); //Aguarda a confirmação da busca do usuário
            if (user == null)
            {
                return BadRequest("Usuário não está cadastrado.");
            }

            var result = await _userManager.AddToRoleAsync(user, roleName); //Aguarda a confirmação da busca da role
            if (result.Succeeded)
            {
                return Ok($"O cargo '{roleName}' foi adicionado ao usuário {user.UserName} com sucesso.");
            }
            else
            {
                return BadRequest(result.Errors);
            }

        }

        [HttpGet("GetUsers")] //Listar todos os Usuários
        public async Task<IActionResult> GetUsers()
        {
            var users = _userManager.Users.ToList();
            if (users == null)
                return NotFound("Nenhum usuário encontrado.");
            return Ok(users);

        }

        [Authorize]
        [HttpGet("GetUsersByRole")]
        public async Task<IActionResult> GetUsersById(string roleName)
        {
            var users = await _userManager.GetUsersInRoleAsync(roleName);
            if (users == null)
                return NotFound($"Nenhum usuário registrado como '{roleName}'.");
            return Ok(users);
        }

        // [HttpPOST] : Criar um novo usuário
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] Usuario usuario, string password)
        {
            if (usuario == null || string.IsNullOrEmpty(password))
                return BadRequest("Dados do usuário ou senha não podem ser nulos.");

            // Verifica se o email já está em uso
            var existingUser = await _userManager.FindByEmailAsync(usuario.Email);
            if (existingUser != null)
                return BadRequest("Já existe um usuário com este email.");

            // Cria o novo usuário
            var newUser = new Usuario
            {
                UserName = usuario.UserName,
                Email = usuario.Email,
                NormalizedEmail = usuario.Email.ToUpper(),
                NormalizedUserName = usuario.UserName.ToUpper(),
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                PhoneNumber = usuario.PhoneNumber,
                NomeCompleto = usuario.NomeCompleto,
                DataNascimento = usuario.DataNascimento
            };

            // Adiciona o usuário ao banco de dados
            var result = await _userManager.CreateAsync(newUser, password);
            if (result.Succeeded)
                return Ok("Usuário criado com sucesso!");

            return BadRequest(result.Errors);
        }

        // Post: Upload da foto de perfil
        [HttpPost("UploadProfilePicture")]
        public async Task<IActionResult> UploadProfilePicture(IFormFile file, string userId)
        {
            // Verifica se o arquivo é nulo ou vazio
            if (file == null || file.Length == 0)
                return BadRequest("Arquivo não pode ser nulo ou vazio.");

            // Verifica se o usuário existe
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("Usuário não encontrado.");

            // Verifica se o arquivo é uma imagem
            if (!file.ContentType.StartsWith("image/"))
                return BadRequest("O arquivo deve ser uma imagem.");

            // Define o caminho para salvar a imagem na pasta Resources/Profile
            var profileFolder = Path.Combine(_webHostEnvironment.ContentRootPath, "Resources", "Profile");
            if (!Directory.Exists(profileFolder))
                Directory.CreateDirectory(profileFolder);

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (Array.IndexOf(allowedExtensions, fileExtension) < 0)
                return BadRequest("Formato de arquivo não suportado. Use .jpg, .jpeg, .png ou .gif.");

            var fileName = $"{user.Id}{fileExtension}";
            var filePath = Path.Combine(profileFolder, fileName);


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
            var relativePath = Path.Combine("Resources", "Profile", fileName).Replace("\\", "/");
            return Ok(new { FilePath = relativePath });
        }

        //Get : Buscar a imagem de perfil do usuário e retornar como Base64
        [HttpGet("GetProfilePicture/{userId}")]
        public async Task<IActionResult> GetProfilePicture(string userId)
        {
            // Verifica se o usuário existe
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("Usuário não encontrado.");

            // Caminho da imagem na pasta Resources/Profile
            var profileFolder = Path.Combine(_webHostEnvironment.ContentRootPath, "Resources", "Profile");
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

            // Procura a imagem do usuário com base no ID
            string? userImagePath = null;
            foreach (var extension in allowedExtensions)
            {
                var potentialPath = Path.Combine(profileFolder, $"{user.Id}{extension}");
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



        // Atualizar cadastro do usuário no login.
        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] Usuario updatedUser)
        {
            var userName = User?.Identity.Name; // Obtém o nome de usuário do token JWT
            if (string.IsNullOrEmpty(userName))
                return BadRequest("Usuário não encontrado.");

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                return NotFound("Usuário não encontrado.");

            user.NomeCompleto = updatedUser.NomeCompleto ?? user.NomeCompleto;
            user.DataNascimento = updatedUser.DataNascimento;
            user.PhoneNumber = updatedUser.PhoneNumber ?? user.PhoneNumber;
            user.Email = updatedUser.Email ?? user.Email;
            user.UserName = updatedUser.UserName ?? user.UserName;
            user.NormalizedUserName = updatedUser.UserName?.ToUpper() ?? user.NormalizedUserName;
            user.NormalizedEmail = updatedUser.Email?.ToUpper() ?? user.NormalizedEmail;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return Ok("Usuário atualizado com sucesso!");
            return BadRequest(result.Errors);

        }





        //Buscar um usuário por Id
        [HttpGet("GetUserById")]
        public async Task<IActionResult> getUserById(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest("ID do usuário não pode ser nulo ou vazio.");
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("Usuário não encontrado.");
            return Ok(user);
        }


        //Adicionar uma forma de pagamento
        [Authorize]
        [HttpPost("AdicionarCartao")]
        public async Task<IActionResult> AdicionarCartao([FromBody] Cartao cartao, string userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Retorna os erros de validação
            }

            // Verifica se o usuário existe
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            // Verifica se o cartão já existe (mesmas informações)
            var cartaoExistente = await _context.Cartoes.FirstOrDefaultAsync(c =>
                c.numeroCartao == cartao.numeroCartao &&
                c.NomeTitular == cartao.NomeTitular &&
                c.dataValidade == cartao.dataValidade &&
                c.codSeguranca == cartao.codSeguranca);

            if (cartaoExistente == null)
            {
                _context.Cartoes.Add(cartao); // Adiciona o cartão no banco de dados
                await _context.SaveChangesAsync(); // Salva as mudanças
                cartaoExistente = cartao; // O cartão adicionado será o existente atual
            }

            // Verifica se o vínculo entre o usuário e o cartão já existe
            var usuarioCartaoExistente = await _context.UsuarioCartoes.AnyAsync(uc =>
                uc.UsuarioId == Guid.Parse(userId) && uc.CartaoId == cartaoExistente.CartaoId);

            if (usuarioCartaoExistente)
            {
                return BadRequest("O cartão já foi vinculado a este usuário.");
            }

            // Cria o vínculo do cartão com o usuário
            var cartaoUsuario = new UsuarioCartao
            {
                UsuarioId = Guid.Parse(userId),
                CartaoId = cartaoExistente.CartaoId
            };

            _context.UsuarioCartoes.Add(cartaoUsuario); // Adiciona o vínculo no banco de dados
            await _context.SaveChangesAsync(); // Salva a mudança

            return Ok("Cartão cadastrado com sucesso!"); // Confirma o cadastro do cartão
        }




    }
}
