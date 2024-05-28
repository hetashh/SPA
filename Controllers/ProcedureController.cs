using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SPA.Data;
using SPA.Models;
using System;
using System.Linq;
using System.Collections.Generic;

namespace SPA.Controllers
{
    public class ProcedureController : Controller
    {
        private readonly SpaContext _context;

        public ProcedureController(SpaContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var procedures = _context.Procedures.ToList();
            return View(procedures);
        }

        [HttpPost]
        public IActionResult Book(int id, DateTime bookingTime)
        {
            DateTime currentTime = DateTime.Now;

            if (bookingTime < currentTime)
            {
                TempData["ErrorMessage"] = "Вы не можете забронировать время, которое уже прошло.";
                return RedirectToAction("Index");
            }

            if (bookingTime.Hour < 10 || bookingTime.Hour > 18 || bookingTime.Minute != 0)
            {
                TempData["ErrorMessage"] = "Время бронирования должно быть с 10:00 до 19:00 и в целый час.";
                return RedirectToAction("Index");
            }

            if (bookingTime > currentTime.AddMonths(1))
            {
                TempData["ErrorMessage"] = "Вы можете бронировать только на месяц вперед.";
                return RedirectToAction("Index");
            }

            var procedure = _context.Procedures.Find(id);
            if (procedure != null)
            {
                var userName = HttpContext.Session.GetString("UserName");
                if (userName != null)
                {
                    var user = _context.Users.SingleOrDefault(u => u.UserName == userName);
                    if (user != null)
                    {
                        var existingBooking = _context.Bookings
                            .Where(b => b.BookingTime == bookingTime && b.ProcedureId == id)
                            .SingleOrDefault();

                        if (existingBooking != null)
                        {
                            TempData["ErrorMessage"] = "Выбранное время уже забронировано.";
                            return RedirectToAction("Index");
                        }

                        var booking = new Booking
                        {
                            ProcedureId = id,
                            BookingTime = bookingTime,
                            UserId = user.Id
                        };

                        _context.Bookings.Add(booking);
                        _context.SaveChanges();

                        TempData["SuccessMessage"] = "Бронирование успешно!";
                        return RedirectToAction("Index");
                    }
                }
            }
            return NotFound();
        }

        [HttpGet]
        public JsonResult GetAvailableTimes(string date, int procedureId)
        {
            DateTime selectedDate = DateTime.Parse(date);
            var bookings = _context.Bookings
                .Where(b => b.BookingTime.Date == selectedDate.Date && b.ProcedureId == procedureId)
                .Select(b => b.BookingTime.Hour)
                .ToList();

            var availableHours = new List<int>();
            for (int hour = 10; hour <= 18; hour++)
            {
                if (!bookings.Contains(hour))
                {
                    availableHours.Add(hour);
                }
            }

            return Json(availableHours);
        }
    }
}
