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

            ViewBag.Procedures = procedures; // Add procedures to ViewBag for display in the view
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult CreateProcedure()
        {
            return View(new Procedure());
        }


        [HttpPost]
        public IActionResult CreateProcedure(Procedure procedure)
        {
            if (ModelState.IsValid)
            {
                _context.Procedures.Add(procedure);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Процедура успешно добавлена.";
                return RedirectToAction("Index");
            }
            return View(procedure);
        }

        [HttpGet]
        public IActionResult EditProcedure(int id)
        {
            var procedure = _context.Procedures.Find(id);
            if (procedure == null)
            {
                return NotFound();
            }
            return View(procedure);
        }

        [HttpPost]
        public IActionResult EditProcedure(Procedure procedure)
        {
            if (ModelState.IsValid)
            {
                _context.Procedures.Update(procedure);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Процедура успешно обновлена.";
                return RedirectToAction("Index");
            }
            return View(procedure);
        }

        [HttpPost]
        public IActionResult DeleteProcedure(int id)
        {
            var procedure = _context.Procedures.Find(id);
            if (procedure != null)
            {
                _context.Procedures.Remove(procedure);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Процедура успешно удалена.";
            }
            return RedirectToAction("Index");
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