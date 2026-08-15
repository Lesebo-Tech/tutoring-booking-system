using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TutoringBookingSystem.Models
{
    public class Student
    {
        public int StudentId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Contact { get; set; }   // WhatsApp number or email

        [Required]
        public string Subject { get; set; }

        public string Grade { get; set; }

        public ICollection<Booking> Bookings { get; set; }
    }
}