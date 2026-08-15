using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;
using TutoringBookingSystem.Models;
using TutoringBookingSystem.Services;

namespace TutoringBookingSystem.Controllers
{
    [Authorize]
    public class BookingsController : Controller
    {
        private TutoringContext db = new TutoringContext();
        private EmailService emailService = new EmailService();

        // GET: Bookings (tutor-facing list of all bookings)
        public ActionResult Index()
        {
            var bookings = db.Bookings
                              .Include(b => b.Student)
                              .Include(b => b.TimeSlot)
                              .OrderByDescending(b => b.CreatedAt)
                              .ToList();
            return View(bookings);
        }



        [AllowAnonymous]
        // GET: Bookings/Create (student-facing booking form)
        public ActionResult Create()
        {
            // Only show slots that are NOT booked yet
            ViewBag.TimeSlotId = new SelectList(
                db.TimeSlots.Where(s => !s.IsBooked).OrderBy(s => s.Date).ThenBy(s => s.StartTime),
                "TimeSlotId", "Date"
            );
            return View();
        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]

        public ActionResult Create(string Name, string Contact, string Subject, string Grade, int TimeSlotId)
        {
            // Re-check the slot right before booking — this is the actual double-booking guard
            var slot = db.TimeSlots.Find(TimeSlotId);

            if (slot == null || slot.IsBooked)
            {
                ModelState.AddModelError("", "Sorry, that slot was just booked by someone else. Please choose another.");
                ViewBag.TimeSlotId = new SelectList(
                    db.TimeSlots.Where(s => !s.IsBooked).OrderBy(s => s.Date).ThenBy(s => s.StartTime),
                    "TimeSlotId", "Date"
                );
                return View();
            }

            // Create the student record
            var student = new Student
            {
                Name = Name,
                Contact = Contact,
                Subject = Subject,
                Grade = Grade
            };
            db.Students.Add(student);
            db.SaveChanges(); // save now so student.StudentId is generated

            // Create the booking
            var booking = new Booking
            {
                StudentId = student.StudentId,
                TimeSlotId = slot.TimeSlotId,
                Status = "Pending",
                PaymentStatus = "Unpaid",
                CreatedAt = DateTime.Now
            };
            db.Bookings.Add(booking);

            // Lock the slot so nobody else can book it
            slot.IsBooked = true;

            db.SaveChanges();

            return RedirectToAction("Confirmation", new { id = booking.BookingId });
        }


        [AllowAnonymous]
        // GET: Bookings/Confirmation/5
        public ActionResult Confirmation(int id)
        {
            var booking = db.Bookings.Find(id);
            if (booking == null) return HttpNotFound();
            return View(booking);
        }

        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MarkPaid(int id)
        {
            var booking = db.Bookings.Find(id);
            if (booking != null)
            {
                booking.PaymentStatus = "Paid";
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateStatus(int id, string status)
        {
            var booking = db.Bookings
                             .Include(b => b.Student)
                             .Include(b => b.TimeSlot)
                             .FirstOrDefault(b => b.BookingId == id);

            if (booking != null)
            {
                booking.Status = status;

                // If cancelled, free up the slot again
                if (status == "Cancelled")
                {
                    var slot = db.TimeSlots.Find(booking.TimeSlotId);
                    if (slot != null) slot.IsBooked = false;
                }

                db.SaveChanges();

                // Send email notification
                if (status == "Confirmed")
                {
                    emailService.SendBookingEmail(
                        booking.Student.Contact,
                        "Your tutoring session is confirmed!",
                        $"Hi {booking.Student.Name}, your session on {booking.TimeSlot.Date.ToShortDateString()} " +
                        $"at {booking.TimeSlot.StartTime} has been confirmed. See you then!"
                    );
                }
                else if (status == "Cancelled")
                {
                    emailService.SendBookingEmail(
                        booking.Student.Contact,
                        "Your tutoring session was cancelled",
                        $"Hi {booking.Student.Name}, unfortunately your session on {booking.TimeSlot.Date.ToShortDateString()} has been cancelled."
                    );
                }
            }

            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult Analytics()
        {
            var bookings = db.Bookings
                              .Include(b => b.Student)
                              .Include(b => b.TimeSlot)
                              .ToList();

            var totalBookings = bookings.Count;
            var confirmedCount = bookings.Count(b => b.Status == "Confirmed");
            var cancelledCount = bookings.Count(b => b.Status == "Cancelled");
            var pendingCount = bookings.Count(b => b.Status == "Pending");

            var totalRevenueSessions = bookings.Count(b => b.PaymentStatus == "Paid");
            var unpaidSessions = bookings.Count(b => b.PaymentStatus == "Unpaid" && b.Status == "Confirmed");

            var subjectBreakdown = bookings
    .GroupBy(b => b.Student.Subject)
    .Select(g => new SubjectStat { Subject = g.Key, Count = g.Count() })
    .OrderByDescending(g => g.Count)
    .ToList();

            var thisMonth = DateTime.Now.Month;
            var thisYear = DateTime.Now.Year;
            var sessionsThisMonth = bookings.Count(b =>
                b.TimeSlot.Date.Month == thisMonth &&
                b.TimeSlot.Date.Year == thisYear &&
                b.Status == "Confirmed");

            ViewBag.TotalBookings = totalBookings;
            ViewBag.ConfirmedCount = confirmedCount;
            ViewBag.CancelledCount = cancelledCount;
            ViewBag.PendingCount = pendingCount;
            ViewBag.PaidSessions = totalRevenueSessions;
            ViewBag.UnpaidSessions = unpaidSessions;
            ViewBag.SubjectBreakdown = subjectBreakdown;
            ViewBag.SessionsThisMonth = sessionsThisMonth;


            // Bookings per month (last 6 months) — for the line chart
            var last6Months = Enumerable.Range(0, 6)
                .Select(i => DateTime.Now.AddMonths(-i))
                .OrderBy(d => d)
                .ToList();

            var monthlyLabels = last6Months.Select(d => d.ToString("MMM")).ToList();
            var monthlyCounts = last6Months.Select(d =>
                bookings.Count(b => b.TimeSlot.Date.Month == d.Month && b.TimeSlot.Date.Year == d.Year)
            ).ToList();

            // Status breakdown — for the bar chart
            var statusLabels = new List<string> { "Pending", "Confirmed", "Completed", "Cancelled" };
            var statusCounts = statusLabels.Select(s => bookings.Count(b => b.Status == s)).ToList();

            ViewBag.MonthlyLabels = Newtonsoft.Json.JsonConvert.SerializeObject(monthlyLabels);
            ViewBag.MonthlyCounts = Newtonsoft.Json.JsonConvert.SerializeObject(monthlyCounts);
            ViewBag.StatusLabels = Newtonsoft.Json.JsonConvert.SerializeObject(statusLabels);
            ViewBag.StatusCounts = Newtonsoft.Json.JsonConvert.SerializeObject(statusCounts);
            ViewBag.SubjectLabelsJson = Newtonsoft.Json.JsonConvert.SerializeObject(subjectBreakdown.Select(s => s.Subject).ToList());
            ViewBag.SubjectCountsJson = Newtonsoft.Json.JsonConvert.SerializeObject(subjectBreakdown.Select(s => s.Count).ToList());


            return View();
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}