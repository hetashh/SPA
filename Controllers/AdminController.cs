using Microsoft.AspNetCore.Mvc;
using SPA.Data;
using SPA.Models;
using SPA.ViewModels;
using System.Linq;

namespace SPA.Controllers
{
    public class AdminController : Controller
    {
        private readonly SpaContext _context;

        public AdminController(SpaContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var users = _context.Users.ToList();
            var bookings = _context.Bookings.ToList();
            var procedures = _context.Procedures.ToList();

            var viewModel = users.Select(user => new AdminDashboardViewModel
            {
                UserName = user.UserName,
                Balance = user.Balance,
                Bookings = bookings
                    .Where(b => b.UserId == user.Id)
                    .Select(b => new BookingViewModel
                    {
                        BookingId = b.Id,
                        ProcedureName = procedures.FirstOrDefault(p => p.Id == b.ProcedureId)?.Name,
                        BookingTime = b.BookingTime.ToString("yyyy-MM-dd HH:mm")
                    }).ToList()
            }).ToList();

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult CancelBooking(int bookingId)
        {
            var booking = _context.Bookings.Find(bookingId);
            if (booking != null)
            {
                var user = _context.Users.Find(booking.UserId);
                var procedure = _context.Procedures.Find(booking.ProcedureId);

                if (user != null && procedure != null)
                {
                    user.Balance += procedure.Price;
                    _context.Bookings.Remove(booking);
                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Бронирование отменено.";
                }
            }
            return RedirectToAction("Index");
        }
    }
}
