﻿using System.ComponentModel.DataAnnotations;

namespace SmartCapital.WebAPI.DTO.Login
{
    public class UserLoginRequest
    {
        [Required(ErrorMessage = "O Nome do Usuário não pode ser vazio.")]
        [StringLength(255, ErrorMessage = "O Nome do Usuário não pode exceder {0} caracteres.")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "A Senha do Usuário não pode ser vazia.")]
        [StringLength(255, ErrorMessage = "A Senha do Usuário não pode ser vazia.")]
        public string UserPassword { get; set; } = null!;
    }
}
