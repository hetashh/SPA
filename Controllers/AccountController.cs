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

        public AccountController(SpaContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.SingleOrDefault(u => u.UserName == model.UserName && u.Password == model.Password);
                if (user != null)
                {
                    HttpContext.Session.SetString("UserName", user.UserName);
                    HttpContext.Session.SetString("UserRole", user.Role);

                    if (user.Role == "Admin")
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Неуачная попытка входа в систему.");
            }

            return View(model);
        }


        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(u => u.UserName == model.UserName))
                {
                    ModelState.AddModelError("", "Имя пользователя уже существует.");
                    return View(model);
                }

                var user = new User
                {
                    UserName = model.UserName,
                    Password = model.Password,
                    Balance = 1000, // Initial balance for testing
                    Role = "User"
                };

                _context.Users.Add(user);
                _context.SaveChanges();

                return RedirectToAction("Login");
            }

            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserName");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Dashboard()
        {
            var userName = HttpContext.Session.GetString("UserName");
            if (userName != null)
            {
                var user = _context.Users.SingleOrDefault(u => u.UserName == userName);
                if (user != null)
                {
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
            return RedirectToAction("Login");
        }

    }
}
