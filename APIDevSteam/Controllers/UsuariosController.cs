using APIDevSteam.Migrations;
using APIDevSteam.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        //Método Construtor com as injeções de dependência

        public UsuariosController(UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _webHostEnvironment = webHostEnvironment;
        }


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

    }
}
