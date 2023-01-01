﻿using Microsoft.AspNetCore.Identity;

namespace WebApiAutores.Entidades
{
    public class Usuario: IdentityUser
    {
        public bool NoHaPagado { get; set; }
    }
}
