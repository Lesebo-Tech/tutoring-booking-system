using System;
using System.Linq;
using System.Web.Mvc;
using TutoringBookingSystem.Models;

namespace TutoringBookingSystem.Controllers
{
    [Authorize]
    public class TimeSlotsController : Controller
    {
        private TutoringContext db = new TutoringContext();

        // GET: TimeSlots
        public ActionResult Index()
        {
            var slots = db.TimeSlots
                .OrderBy(s => s.Date)
                .ThenBy(s => s.StartTime)
                .ToList();

            return View(slots);
        }

        // GET: TimeSlots/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TimeSlots/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TimeSlot slot)
        {
            // Check that the session has valid start and end times.
            if (slot.EndTime <= slot.StartTime)
            {
                ModelState.AddModelError(
                    "",
                    "End time must be after start time."
                );
            }

            // Check whether another session overlaps this time.
            if (ModelState.IsValid)
            {
                bool overlaps = db.TimeSlots.Any(s =>
                    s.Date == slot.Date &&
                    s.StartTime < slot.EndTime &&
                    s.EndTime > slot.StartTime
                );

                if (overlaps)
                {
                    ModelState.AddModelError(
                        "",
                        "This time slot overlaps an existing session. Please choose another time."
                    );
                }
            }

            if (ModelState.IsValid)
            {
                // Every new time slot starts as available.
                slot.IsBooked = false;

                db.TimeSlots.Add(slot);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(slot);
        }

        // GET: TimeSlots/Delete/5
        public ActionResult Delete(int id)
        {
            var slot = db.TimeSlots.Find(id);

            if (slot == null)
            {
                return HttpNotFound();
            }

            return View(slot);
        }

        // POST: TimeSlots/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var slot = db.TimeSlots.Find(id);

            if (slot == null)
            {
                return HttpNotFound();
            }

            db.TimeSlots.Remove(slot);
            db.SaveChanges();

            return RedirectToAction("Index");
        }


        // GET: TimeSlots/Calendar
        public ActionResult Calendar(DateTime? weekStart)
        {
            // Use the current week if no date is provided.
            DateTime selectedDate =
                (weekStart ?? DateTime.Today).Date;

            // Calculate Monday of the selected week.
            int daysFromMonday =
                ((int)selectedDate.DayOfWeek + 6) % 7;

            DateTime monday =
                selectedDate.AddDays(-daysFromMonday);

            DateTime nextMonday = monday.AddDays(7);

            // Retrieve sessions for the selected week.
            var slots = db.TimeSlots
                .Where(s => s.Date >= monday &&
                            s.Date < nextMonday)
                .OrderBy(s => s.Date)
                .ThenBy(s => s.StartTime)
                .ToList();

            // Send the selected week to the Razor view.
            ViewBag.WeekStart = monday;
            ViewBag.PreviousWeek = monday.AddDays(-7);
            ViewBag.NextWeek = monday.AddDays(7);

            return View(slots);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
