namespace SPA.ViewModels
{
    public class DashboardViewModel
    {
        public decimal Balance { get; set; }
        public List<BookingViewModel> Bookings { get; set; }
    }

    public class BookingViewModel
    {
        public int BookingId { get; set; } // Добавляем поле BookingId
        public string ProcedureName { get; set; }
        public string BookingTime { get; set; }
    }
}
