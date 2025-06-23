using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using venueBooking.Data;
using venueBooking.Models;

namespace venueBooking.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var events = _context.Events
                .Include(e => e.Venue)
                .Include(e => e.EventType);
            return View(await events.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var evt = await _context.Events
                .Include(e => e.Venue)
                .Include(e => e.EventType)
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (evt == null) return NotFound();

            return View(evt);
        }

        public IActionResult Create()
        {
            ViewBag.Venues = new SelectList(_context.Venues, "VenueId", "VenueName");
            ViewBag.EventTypes = new SelectList(_context.EventTypes, "EventTypeId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event evt)
        {
            if (ModelState.IsValid)
            {
                _context.Add(evt);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Venues = new SelectList(_context.Venues, "VenueId", "VenueName", evt.VenueId);
            ViewBag.EventTypes = new SelectList(_context.EventTypes, "EventTypeId", "Name", evt.EventTypeId);
            return View(evt);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var evt = await _context.Events.FindAsync(id);
            if (evt == null) return NotFound();

            ViewBag.Venues = new SelectList(_context.Venues, "VenueId", "VenueName", evt.VenueId);
            ViewBag.EventTypes = new SelectList(_context.EventTypes, "EventTypeId", "Name", evt.EventTypeId);
            return View(evt);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Event evt)
        {
            if (id != evt.EventId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(evt);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(evt.EventId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Venues = new SelectList(_context.Venues, "VenueId", "VenueName", evt.VenueId);
            ViewBag.EventTypes = new SelectList(_context.EventTypes, "EventTypeId", "Name", evt.EventTypeId);
            return View(evt);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var evt = await _context.Events
                .Include(e => e.Venue)
                .Include(e => e.EventType)
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (evt == null) return NotFound();

            return View(evt);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var evt = await _context.Events.FindAsync(id);
            if (evt != null)
            {
                _context.Events.Remove(evt);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventId == id);
        }
    }
}
