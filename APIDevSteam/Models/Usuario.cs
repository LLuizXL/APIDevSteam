﻿using Microsoft.AspNetCore.Identity;

namespace APIDevSteam.Models
{
    public class Usuario : IdentityUser
    {
        public Usuario() : base()
        {
        }
        public DateOnly DataNascimento { get; set; }
        public string? NomeCompleto { get; set; }

        // Lista de jogos comprados pelo usuário
        public ICollection<UsuarioJogo> JogosComprados { get; set; } = new List<UsuarioJogo>();
    }
}
