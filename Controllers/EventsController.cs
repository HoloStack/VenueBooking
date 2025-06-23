using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using venueBooking.Data;
using venueBooking.Models;

namespace venueBooking.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public EventsController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: Events
        // filter by type, date range
        public async Task<IActionResult> Index(int? eventTypeId, DateTime? fromDate, DateTime? toDate)
        {
            var query = _db.Events
                .Include(e => e.Venue)
                .Include(e => e.EventType)
                .AsQueryable();

            if (eventTypeId.HasValue)
                query = query.Where(e => e.EventTypeId == eventTypeId.Value);
            if (fromDate.HasValue)
                query = query.Where(e => e.EventDate.Date >= fromDate.Value.Date);
            if (toDate.HasValue)
                query = query.Where(e => e.EventDate.Date <= toDate.Value.Date);

            ViewBag.EventTypes = new SelectList(await _db.EventTypes.ToListAsync(), "EventTypeId", "Name", eventTypeId);
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

            var events = await query.OrderBy(e => e.EventDate).ToListAsync();
            return View(events);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            ViewBag.Venues = new SelectList(_db.Venues, "VenueId", "VenueName");
            ViewBag.EventTypes = new SelectList(_db.EventTypes, "EventTypeId", "Name");
            return View();
        }

        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event evt)
        {
            // prevent past dates
            if (evt.EventDate < DateTime.Now)
                ModelState.AddModelError(nameof(evt.EventDate), "Event date cannot be in the past.");

            // one event per venue per day
            bool conflict = await _db.Events
                .AnyAsync(e => e.VenueId == evt.VenueId && e.EventDate.Date == evt.EventDate.Date);
            if (conflict)
                ModelState.AddModelError(string.Empty, "Only one event per venue per day is allowed.");

            if (!ModelState.IsValid)
            {
                ViewBag.Venues = new SelectList(_db.Venues, "VenueId", "VenueName", evt.VenueId);
                ViewBag.EventTypes = new SelectList(_db.EventTypes, "EventTypeId", "Name", evt.EventTypeId);
                return View(evt);
            }

            _db.Events.Add(evt);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var evt = await _db.Events.FindAsync(id);
            if (evt == null) return NotFound();
            ViewBag.Venues = new SelectList(_db.Venues, "VenueId", "VenueName", evt.VenueId);
            ViewBag.EventTypes = new SelectList(_db.EventTypes, "EventTypeId", "Name", evt.EventTypeId);
            return View(evt);
        }

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Event evt)
        {
            if (id != evt.EventId) return NotFound();
            if (evt.EventDate < DateTime.Now)
                ModelState.AddModelError(nameof(evt.EventDate), "Event date cannot be in the past.");

            bool conflict = await _db.Events
                .Where(e => e.EventId != id)
                .AnyAsync(e => e.VenueId == evt.VenueId && e.EventDate.Date == evt.EventDate.Date);
            if (conflict)
                ModelState.AddModelError(string.Empty, "Only one event per venue per day is allowed.");

            if (!ModelState.IsValid)
            {
                ViewBag.Venues = new SelectList(_db.Venues, "VenueId", "VenueName", evt.VenueId);
                ViewBag.EventTypes = new SelectList(_db.EventTypes, "EventTypeId", "Name", evt.EventTypeId);
                return View(evt);
            }

            try
            {
                _db.Update(evt);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_db.Events.Any(e => e.EventId == id))
                    return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var evt = await _db.Events
                .Include(e => e.Venue)
                .Include(e => e.EventType)
                .FirstOrDefaultAsync(e => e.EventId == id);
            if (evt == null) return NotFound();
            return View(evt);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var evt = await _db.Events.FindAsync(id);
            if (evt != null)
            {
                _db.Events.Remove(evt);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}