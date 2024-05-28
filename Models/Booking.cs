using System;

namespace SPA.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int ProcedureId { get; set; }
        public Procedure Procedure { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime BookingTime { get; set; }
    }
}
