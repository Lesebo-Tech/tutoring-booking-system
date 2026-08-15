using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace TutoringBookingSystem.Models
{
    public class TutoringContext : DbContext
    {
        public TutoringContext() : base("TutoringConnectionString")
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<TimeSlot> TimeSlots { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Tutor> Tutors { get; set; }
    }
}