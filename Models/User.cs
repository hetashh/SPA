﻿namespace SPA.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public decimal Balance { get; set; }
        public string Role { get; set; } // Добавляем поле для роли
    }

}
