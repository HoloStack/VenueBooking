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
        // /Events?eventTypeId=2&fromDate=2025-06-20&toDate=2025-06-25
        public async Task<IActionResult> Index(int? eventTypeId, DateTime? fromDate, DateTime? toDate)
        {
            var q = _db.Events
                .Include(e => e.Venue)
                .Include(e => e.EventType)
                .AsQueryable();

            if (eventTypeId.HasValue)
                q = q.Where(e => e.EventTypeId == eventTypeId.Value);

            if (fromDate.HasValue)
                q = q.Where(e => e.EventDate.Date >= fromDate.Value.Date);

            if (toDate.HasValue)
                q = q.Where(e => e.EventDate.Date <= toDate.Value.Date);

            ViewBag.EventTypesFilter = new SelectList(
                await _db.EventTypes.ToListAsync(), 
                "EventTypeId", 
                "Name", 
                eventTypeId);

            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate   = toDate?.ToString("yyyy-MM-dd");

            var list = await q.OrderBy(e => e.EventDate).ToListAsync();
            return View(list);
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var evt = await _db.Events
                .Include(e => e.Venue)
                .Include(e => e.EventType)
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (evt == null) return NotFound();
            return View(evt);
        }

        // GET: Events/Create
        public IActionResult Create(int? venueId, string venueName)
        {
            // Create a new event model
            var evt = new Event();
            
            // If venueId is provided, pre-select it
            if (venueId.HasValue)
            {
                evt.VenueId = venueId.Value;
                
                // Load event types for the selected venue
                var venue = _db.Venues
                    .Include(v => v.SupportedEventTypes)
                    .FirstOrDefault(v => v.VenueId == venueId.Value);
                var supported = venue?.SupportedEventTypes ?? new List<EventType>();
                
                ViewBag.EventTypes = new SelectList(supported, "EventTypeId", "Name");
                ViewBag.PreSelectedVenue = venueName;
            }
            else
            {
                ViewBag.EventTypes = new SelectList(Enumerable.Empty<EventType>(), "EventTypeId", "Name");
            }
            
            ViewBag.Venues = new SelectList(_db.Venues, "VenueId", "VenueName", venueId);
            return View(evt);
        }

        // API endpoint to get event types for a specific venue
        [HttpGet]
        public IActionResult GetEventTypesForVenue(int venueId)
        {
            var venue = _db.Venues
                .Include(v => v.SupportedEventTypes)
                .FirstOrDefault(v => v.VenueId == venueId);
            
            var eventTypes = venue?.SupportedEventTypes ?? new List<EventType>();
            
            return Json(eventTypes.Select(et => new { 
                Value = et.EventTypeId, 
                Text = et.Name 
            }));
        }

        // Test page for AJAX debugging
        public IActionResult TestAjax()
        {
            return View();
        }

        // POST: Events/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event evt)
        {
            // Past-date guard
            if (evt.EventDate.Date < DateTime.Today)
                ModelState.AddModelError(nameof(evt.EventDate), "You cannot book an event in the past.");

            // Double-booking guard
            bool conflict = await _db.Events
                .AnyAsync(e => e.VenueId == evt.VenueId
                            && e.EventDate.Date == evt.EventDate.Date);
            if (conflict)
                ModelState.AddModelError("", "That venue already has an event on this date.");

            if (ModelState.IsValid)
            {
                _db.Add(evt);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // on failure: re-populate dropdowns
            ViewBag.Venues = new SelectList(
                _db.Venues, "VenueId", "VenueName", evt.VenueId);

            // only the types supported by that venue
            var venue = _db.Venues
                .Include(v => v.SupportedEventTypes)
                .FirstOrDefault(v => v.VenueId == evt.VenueId);
            var supported = venue?.SupportedEventTypes ?? new List<EventType>();
            ViewBag.EventTypes = new SelectList(
                supported, "EventTypeId", "Name", evt.EventTypeId);

            return View(evt);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var evt = await _db.Events.FindAsync(id);
            if (evt == null) return NotFound();

            ViewBag.Venues = new SelectList(
                _db.Venues, "VenueId", "VenueName", evt.VenueId);

            var venue = _db.Venues
                .Include(v => v.SupportedEventTypes)
                .FirstOrDefault(v => v.VenueId == evt.VenueId);
            var supported = venue?.SupportedEventTypes ?? new List<EventType>();
            ViewBag.EventTypes = new SelectList(
                supported, "EventTypeId", "Name", evt.EventTypeId);

            return View(evt);
        }

        // POST: Events/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Event evt)
        {
            if (id != evt.EventId) return NotFound();

            if (evt.EventDate.Date < DateTime.Today)
                ModelState.AddModelError(nameof(evt.EventDate), "You cannot book an event in the past.");

            bool conflict = await _db.Events
                .AnyAsync(e => e.EventId != id
                            && e.VenueId == evt.VenueId
                            && e.EventDate.Date == evt.EventDate.Date);
            if (conflict)
                ModelState.AddModelError("", "That venue already has an event on this date.");

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(evt);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _db.Events.AnyAsync(e => e.EventId == id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Venues = new SelectList(
                _db.Venues, "VenueId", "VenueName", evt.VenueId);
            var venue = _db.Venues
                .Include(v => v.SupportedEventTypes)
                .FirstOrDefault(v => v.VenueId == evt.VenueId);
            var supportedTypes = venue?.SupportedEventTypes ?? new List<EventType>();
            ViewBag.EventTypes = new SelectList(
                supportedTypes, "EventTypeId", "Name", evt.EventTypeId);

            return View(evt);
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
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
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