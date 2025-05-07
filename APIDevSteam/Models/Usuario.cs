using Microsoft.AspNetCore.Identity;

namespace APIDevSteam.Models
{
    public class Usuario : IdentityUser
    {
        public Usuario() : base()
        {
        }
        public DateOnly DataNascimento { get; set; }
        public string? NomeCompleto { get; set; }
        public bool? AdminRole { get; set; }

    }
}
