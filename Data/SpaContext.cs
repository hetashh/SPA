using Microsoft.EntityFrameworkCore;
using SPA.Models;
using System;

namespace SPA.Data
{
    public class SpaContext : DbContext
    {
        // Конструктор контекста, принимает параметры конфигурации через Dependency Injection
        public SpaContext(DbContextOptions<SpaContext> options) : base(options) { }

        // Таблицы базы данных
        public DbSet<User> Users { get; set; }
        public DbSet<Procedure> Procedures { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        // Метод для настройки моделей базы данных
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка типа данных для свойства Price в модели Procedure
            modelBuilder.Entity<Procedure>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18)");

            // Настройка типа данных для свойства Balance в модели User
            modelBuilder.Entity<User>()
                .Property(u => u.Balance)
                .HasColumnType("decimal(18)");

            // Заполнение таблицы Users начальными данными
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, UserName = "john_doe", Password = "password123", Balance = 1000, Role = "User" },
                new User { Id = 2, UserName = "jane_smith", Password = "password456", Balance = 1000, Role = "User" },
                new User { Id = 3, UserName = "admin", Password = "admin123", Balance = 1000, Role = "Admin" }
            );

            // Заполнение таблицы Procedures начальными данными
            modelBuilder.Entity<Procedure>().HasData(
                new Procedure { Id = 1, Name = "Шведский массаж", Description = "Расслабляющий массаж всего тела", Price = 75 },
                new Procedure { Id = 2, Name = "Массаж горячими камнями", Description = "Лечебный массаж горячими камнями", Price = 100 },
                new Procedure { Id = 3, Name = "Ароматерапия", Description = "Успокаивающий массаж с эфирными маслами", Price = 80 }
            );

            // Заполнение таблицы Bookings начальными данными
            modelBuilder.Entity<Booking>().HasData(
                new Booking { Id = 1, ProcedureId = 1, UserId = 1, BookingTime = new DateTime(2024, 6, 1, 14, 0, 0) },
                new Booking { Id = 2, ProcedureId = 2, UserId = 1, BookingTime = new DateTime(2024, 6, 2, 16, 0, 0) },
                new Booking { Id = 3, ProcedureId = 3, UserId = 2, BookingTime = new DateTime(2024, 6, 3, 11, 0, 0) }
            );
        }
    }
}
