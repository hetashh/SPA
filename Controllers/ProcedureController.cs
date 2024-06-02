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
        public IActionResult Book(int id, DateTime bookingDate, string bookingTime)
        {
            var procedure = _context.Procedures.Find(id);
            if (procedure != null)
            {
                var userName = HttpContext.Session.GetString("UserName");
                if (userName != null)
                {
                    var user = _context.Users.SingleOrDefault(u => u.UserName == userName);
                    if (user != null)
                    {
                        DateTime bookingDateTime = bookingDate.Date.Add(TimeSpan.Parse(bookingTime));

                        if (bookingDateTime <= DateTime.Now)
                        {
                            TempData["ErrorMessage"] = "Вы не можете забронировать врем которое уже прошло";
                            return RedirectToAction("Index");
                        }

                        if (_context.Bookings.Any(b => b.ProcedureId == id && b.BookingTime == bookingDateTime))
                        {
                            TempData["ErrorMessage"] = "Это время уже занято";
                            return RedirectToAction("Index");
                        }

                        if (user.Balance < procedure.Price)
                        {
                            TempData["ErrorMessage"] = "У вас недостаточно средств для бронирования";
                            return RedirectToAction("Index");
                        }

                        var booking = new Booking
                        {
                            ProcedureId = id,
                            BookingTime = bookingDateTime,
                            UserId = user.Id
                        };

                        user.Balance -= procedure.Price;
                        _context.Bookings.Add(booking);
                        _context.SaveChanges();

                        TempData["SuccessMessage"] = "Бронирование успешно";
                        return RedirectToAction("Index");
                    }
                }
            }
            return NotFound();
        }


        [HttpPost]
        public IActionResult CancelBooking(int bookingId)
        {
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
                            user.Balance += procedure.Price;
                            _context.Bookings.Remove(booking);
                            _context.SaveChanges();

                            TempData["SuccessMessage"] = "Бронирование отменено";
                            return RedirectToAction("Dashboard", "Account");
                        }
                    }
                }
            }
            return NotFound();
        }


        [HttpGet]
        public JsonResult GetAvailableTimes(DateTime date, int procedureId)
        {
            var times = new List<string>();
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
