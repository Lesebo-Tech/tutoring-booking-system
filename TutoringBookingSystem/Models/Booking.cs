using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TutoringBookingSystem.Models
{
    public class Booking
    {
        public int BookingId { get; set; }

        [Required]
        public int StudentId { get; set; }
        public Student Student { get; set; }

        [Required]
        public int TimeSlotId { get; set; }
        public TimeSlot TimeSlot { get; set; }

        public string Status { get; set; } = "Pending";
        public string PaymentStatus { get; set; } = "Unpaid";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}