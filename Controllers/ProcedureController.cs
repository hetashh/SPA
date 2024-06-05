using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SPA.Data;
using SPA.Models;
using System;
using System.Linq;

namespace SPA.Controllers
{
    public class ProcedureController : Controller
    {
        private readonly SpaContext _context;

        // Конструктор контроллера, принимает контекст базы данных через Dependency Injection
        public ProcedureController(SpaContext context)
        {
            _context = context;
        }

        // Метод для отображения списка всех процедур
        public IActionResult Index()
        {
            var procedures = _context.Procedures.ToList();
            return View(procedures);
        }

        // Метод для обработки бронирования процедуры (POST-запрос)
        [HttpPost]
        public IActionResult Book(int id, DateTime bookingDate, string bookingTime)
        {
            // Проверка наличия времени для бронирования
            if (string.IsNullOrEmpty(bookingTime))
            {
                TempData["ErrorMessage"] = "Выберите время для бронирования.";
                return RedirectToAction("Index");
            }

            // Поиск процедуры по ID
            var procedure = _context.Procedures.Find(id);
            if (procedure != null)
            {
                // Получение имени пользователя из сессии
                var userName = HttpContext.Session.GetString("UserName");
                if (userName != null)
                {
                    // Поиск пользователя в базе данных по имени пользователя
                    var user = _context.Users.SingleOrDefault(u => u.UserName == userName);
                    if (user != null)
                    {
                        // Формирование полного времени бронирования
                        DateTime bookingDateTime = bookingDate.Date.Add(TimeSpan.Parse(bookingTime));

                        // Проверка валидности времени бронирования
                        if (bookingDateTime <= DateTime.Now)
                        {
                            TempData["ErrorMessage"] = "Вы не можете забронировать время, которое уже прошло.";
                            return RedirectToAction("Index");
                        }

                        // Проверка занятости времени
                        if (_context.Bookings.Any(b => b.ProcedureId == id && b.BookingTime == bookingDateTime))
                        {
                            TempData["ErrorMessage"] = "Это время уже занято.";
                            return RedirectToAction("Index");
                        }

                        // Проверка баланса пользователя
                        if (user.Balance < procedure.Price)
                        {
                            TempData["ErrorMessage"] = "У вас недостаточно средств для бронирования.";
                            return RedirectToAction("Index");
                        }

                        // Создание нового бронирования и обновление баланса пользователя
                        var booking = new Booking
                        {
                            ProcedureId = id,
                            BookingTime = bookingDateTime,
                            UserId = user.Id
                        };

                        user.Balance -= procedure.Price;
                        _context.Bookings.Add(booking);
                        _context.SaveChanges();

                        TempData["SuccessMessage"] = "Бронирование успешно!";
                        return RedirectToAction("Index");
                    }
                }
            }
            return NotFound();
        }

        // Метод для обработки отмены бронирования (POST-запрос)
        [HttpPost]
        public IActionResult CancelBooking(int bookingId)
        {
            // Поиск бронирования по ID
            var booking = _context.Bookings.Find(bookingId);
            if (booking != null)
            {
                var userName = HttpContext.Session.GetString("UserName");
                if (userName != null)
                {
                    var user = _context.Users.SingleOrDefault(u => u.UserName == userName);
                    if (user != null && booking.UserId == user.Id)
                    {
                        var procedure = _context.Procedures.Find(booking.ProcedureId);
                        if (procedure != null)
                        {
                            // Возвращение средств пользователю и удаление бронирования
                            user.Balance += procedure.Price;
                            _context.Bookings.Remove(booking);
                            _context.SaveChanges();

                            TempData["SuccessMessage"] = "Бронирование отменено!";
                            return RedirectToAction("Dashboard", "Account");
                        }
                    }
                }
            }
            return NotFound();
        }

        // Метод для получения доступных времен для бронирования процедуры
        [HttpGet]
        public JsonResult GetAvailableTimes(DateTime date, int procedureId)
        {
            var times = new List<string>();
            // Проверка доступности каждого часа с 10:00 до 19:00
            for (int hour = 10; hour < 19; hour++)
            {
                var bookingTime = new DateTime(date.Year, date.Month, date.Day, hour, 0, 0);
                if (!_context.Bookings.Any(b => b.ProcedureId == procedureId && b.BookingTime == bookingTime))
                {
                    times.Add(bookingTime.ToString("HH:mm"));
                }
            }
            return Json(times);
        }
    }
}
