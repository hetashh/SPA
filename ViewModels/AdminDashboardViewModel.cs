using System.Collections.Generic;

namespace SPA.ViewModels
{
    public class AdminDashboardViewModel
    {
        public string UserName { get; set; }
        public decimal Balance { get; set; }
        public List<BookingViewModel> Bookings { get; set; }
    }
}
