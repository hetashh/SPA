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

        // Конструктор контроллера, принимает контекст базы данных через Dependency Injection
        public AdminController(SpaContext context)
        {
            _context = context;
        }

        // Метод для отображения панели администратора
        public IActionResult Index()
        {
            // Получение всех пользователей, бронирований и процедур из базы данных
            var users = _context.Users.ToList();
            var bookings = _context.Bookings.ToList();
            var procedures = _context.Procedures.ToList();

            // Формирование модели представления для панели администратора
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

            // Передача списка процедур во ViewBag для использования в представлении
            ViewBag.Procedures = procedures;
            return View(viewModel);
        }

        // Метод для отображения страницы создания процедуры
        [HttpGet]
        public IActionResult CreateProcedure()
        {
            return View(new Procedure());
        }

        // Метод для обработки данных создания процедуры (POST-запрос)
        [HttpPost]
        public IActionResult CreateProcedure(Procedure procedure)
        {
            // Проверка валидности модели
            if (ModelState.IsValid)
            {
                // Добавление новой процедуры в базу данных и сохранение изменений
                _context.Procedures.Add(procedure);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Процедура успешно добавлена.";
                return RedirectToAction("Index");
            }
            return View(procedure);
        }

        // Метод для отображения страницы редактирования процедуры
        [HttpGet]
        public IActionResult EditProcedure(int id)
        {
            // Поиск процедуры по ID
            var procedure = _context.Procedures.Find(id);
            if (procedure == null)
            {
                return NotFound();
            }
            return View(procedure);
        }

        // Метод для обработки данных редактирования процедуры (POST-запрос)
        [HttpPost]
        public IActionResult EditProcedure(Procedure procedure)
        {
            // Проверка валидности модели
            if (ModelState.IsValid)
            {
                // Обновление процедуры в базе данных и сохранение изменений
                _context.Procedures.Update(procedure);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Процедура успешно обновлена.";
                return RedirectToAction("Index");
            }
            return View(procedure);
        }

        // Метод для обработки удаления процедуры (POST-запрос)
        [HttpPost]
        public IActionResult DeleteProcedure(int id)
        {
            // Поиск процедуры по ID
            var procedure = _context.Procedures.Find(id);
            if (procedure != null)
            {
                // Удаление процедуры из базы данных и сохранение изменений
                _context.Procedures.Remove(procedure);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Процедура успешно удалена.";
            }
            return RedirectToAction("Index");
        }

        // Метод для обработки отмены бронирования (POST-запрос)
        [HttpPost]
        public IActionResult CancelBooking(int bookingId)
        {
            // Поиск бронирования по ID
            var booking = _context.Bookings.Find(bookingId);
            if (booking != null)
            {
                var user = _context.Users.Find(booking.UserId);
                var procedure = _context.Procedures.Find(booking.ProcedureId);

                // Если пользователь и процедура найдены, выполняем отмену бронирования
                if (user != null && procedure != null)
                {
                    user.Balance += procedure.Price; // Возвращаем средства пользователю
                    _context.Bookings.Remove(booking); // Удаляем бронирование
                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Бронирование отменено.";
                }
            }
            return RedirectToAction("Index");
        }
    }
}
