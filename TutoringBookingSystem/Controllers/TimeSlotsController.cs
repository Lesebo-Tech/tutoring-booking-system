using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TutoringBookingSystem.Models;

namespace TutoringBookingSystem.Controllers
{
    public class TimeSlotsController : Controller
    {
        private TutoringContext db = new TutoringContext();

        // GET: TimeSlots
        public ActionResult Index()
        {
            var slots = db.TimeSlots.OrderBy(s => s.Date).ThenBy(s => s.StartTime).ToList();
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
            if (ModelState.IsValid)
            {
                slot.IsBooked = false;   // every new slot starts open
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
                return HttpNotFound();
            return View(slot);
        }

        // POST: TimeSlots/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var slot = db.TimeSlots.Find(id);
            db.TimeSlots.Remove(slot);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}