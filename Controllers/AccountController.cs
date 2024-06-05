using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SPA.Data;
using SPA.Models;
using SPA.ViewModels;
using System;
using System.Linq;

namespace SPA.Controllers
{
    public class AccountController : Controller
    {
        private readonly SpaContext _context;

        // Конструктор контроллера, принимает контекст базы данных через Dependency Injection
        public AccountController(SpaContext context)
        {
            _context = context;
        }

        // Метод для отображения страницы входа
        public IActionResult Login()
        {
            return View();
        }

        // Метод для обработки данных входа (POST-запрос)
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            // Проверка валидности модели
            if (ModelState.IsValid)
            {
                // Поиск пользователя в базе данных по имени пользователя и паролю
                var user = _context.Users.SingleOrDefault(u => u.UserName == model.UserName && u.Password == model.Password);

                // Если пользователь найден, сохраняем данные в сессии
                if (user != null)
                {
                    HttpContext.Session.SetString("UserName", user.UserName);
                    HttpContext.Session.SetString("UserRole", user.Role);

                    // Перенаправление на разные страницы в зависимости от роли пользователя
                    if (user.Role == "Admin")
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    return RedirectToAction("Index", "Home");
                }

                // Если пользователь не найден, добавляем сообщение об ошибке
                ModelState.AddModelError("", "Неверный логин или пароль.");
            }

            // Если модель невалидна или пользователь не найден, возвращаем ту же страницу с моделью
            return View(model);
        }

        // Метод для отображения страницы регистрации
        public IActionResult Register()
        {
            return View();
        }

        // Метод для обработки данных регистрации (POST-запрос)
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            // Проверка валидности модели
            if (ModelState.IsValid)
            {
                // Проверка наличия пользователя с таким же именем в базе данных
                if (_context.Users.Any(u => u.UserName == model.UserName))
                {
                    ModelState.AddModelError("", "Имя пользователя уже существует.");
                    return View(model);
                }

                // Проверка совпадения пароля и его подтверждения
                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("", "Пароли не совпадают.");
                    return View(model);
                }

                // Создание нового пользователя и добавление его в базу данных
                var user = new User
                {
                    UserName = model.UserName,
                    Password = model.Password,
                    Balance = 1000, // Начальный баланс
                    Role = "User" // Роль по умолчанию
                };

                _context.Users.Add(user);
                _context.SaveChanges();

                // Перенаправление на страницу входа после успешной регистрации
                return RedirectToAction("Login");
            }

            // Если модель невалидна, возвращаем ту же страницу с моделью
            return View(model);
        }

        // Метод для выхода пользователя из системы
        public IActionResult Logout()
        {
            // Удаление данных пользователя из сессии
            HttpContext.Session.Remove("UserName");
            return RedirectToAction("Index", "Home");
        }

        // Метод для отображения панели управления пользователя
        public IActionResult Dashboard()
        {
            // Получение имени пользователя из сессии
            var userName = HttpContext.Session.GetString("UserName");
            if (userName != null)
            {
                // Поиск пользователя в базе данных по имени пользователя
                var user = _context.Users.SingleOrDefault(u => u.UserName == userName);
                if (user != null)
                {
                    // Получение бронирований пользователя и создание модели представления
                    var bookings = _context.Bookings
                        .Where(b => b.UserId == user.Id)
                        .Select(b => new BookingViewModel
                        {
                            BookingId = b.Id,
                            ProcedureName = b.Procedure.Name,
                            BookingTime = b.BookingTime.ToString("yyyy-MM-dd HH:mm")
                        }).ToList();

                    var model = new DashboardViewModel
                    {
                        Balance = user.Balance,
                        Bookings = bookings
                    };

                    return View(model);
                }
            }
            // Если пользователь не найден или не авторизован, перенаправляем на страницу входа
            return RedirectToAction("Login");
        }
    }
}
