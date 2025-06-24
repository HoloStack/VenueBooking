using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using venueBooking.Data;
using venueBooking.Models;
using venueBooking.Services;

namespace venueBooking.Controllers
{
    public class VenuesController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly BlobService _blobService;
        public VenuesController(ApplicationDbContext db, BlobService blobService)
        {
            _db = db;
            _blobService = blobService;
        }

        // GET: Venues
        // Optional filters: eventTypeId, date (single)
        public async Task<IActionResult> Index(int? eventTypeId, DateTime? date)
        {
            var query = _db.Venues
                .Include(v => v.SupportedEventTypes)
                .Include(v => v.Events)
                .AsQueryable();

            if (eventTypeId.HasValue)
            {
                query = query.Where(v => v.SupportedEventTypes
                    .Any(et => et.EventTypeId == eventTypeId.Value));
            }

            if (date.HasValue)
            {
                var d = date.Value.Date;
                query = query.Where(v => !v.Events
                    .Any(e => e.EventDate.Date == d));
            }

            var types = await _db.EventTypes.OrderBy(et => et.Name).ToListAsync();
            ViewBag.EventTypesFilter = new SelectList(types, "EventTypeId", "Name", eventTypeId);
            ViewBag.Date = date?.ToString("yyyy-MM-dd");

            var venues = await query.ToListAsync();
            return View(venues);
        }

        // GET: Venues/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _db.Venues
                .Include(v => v.SupportedEventTypes)
                .Include(v => v.Events)
                .FirstOrDefaultAsync(v => v.VenueId == id.Value);
            if (venue == null) return NotFound();

            return View(venue);
        }

        // GET: Venues/Create
        public async Task<IActionResult> Create()
        {
            var types = await _db.EventTypes.OrderBy(et => et.Name).ToListAsync();
            ViewBag.EventTypes = new MultiSelectList(types, "EventTypeId", "Name");
            return View();
        }

        // POST: Venues/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Venue venue, IFormFile image, int[] EventTypeIds)
        {
            if (!ModelState.IsValid)
            {
                var types = await _db.EventTypes.OrderBy(et => et.Name).ToListAsync();
                ViewBag.EventTypes = new MultiSelectList(types, "EventTypeId", "Name", EventTypeIds);
                return View(venue);
            }

            if (image?.Length > 0)
            {
                venue.ImageUrl = await _blobService.UploadAsync(image);
            }

            venue.SupportedEventTypes = await _db.EventTypes
                .Where(et => EventTypeIds.Contains(et.EventTypeId))
                .ToListAsync();

            _db.Venues.Add(venue);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Venues/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _db.Venues
                .Include(v => v.SupportedEventTypes)
                .FirstOrDefaultAsync(v => v.VenueId == id.Value);
            if (venue == null) return NotFound();

            var types = await _db.EventTypes.OrderBy(et => et.Name).ToListAsync();
            ViewBag.EventTypes = new MultiSelectList(types, "EventTypeId", "Name",
                venue.SupportedEventTypes.Select(et => et.EventTypeId));
            return View(venue);
        }

        // POST: Venues/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Venue venue, IFormFile image, int[] EventTypeIds)
        {
            if (id != venue.VenueId) return NotFound();

            if (!ModelState.IsValid)
            {
                var types = await _db.EventTypes.OrderBy(et => et.Name).ToListAsync();
                ViewBag.EventTypes = new MultiSelectList(types, "EventTypeId", "Name", EventTypeIds);
                return View(venue);
            }

            if (image?.Length > 0)
            {
                venue.ImageUrl = await _blobService.UploadAsync(image);
            }

            venue.SupportedEventTypes = await _db.EventTypes
                .Where(et => EventTypeIds.Contains(et.EventTypeId))
                .ToListAsync();

            _db.Venues.Update(venue);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Venues/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _db.Venues
                .Include(v => v.SupportedEventTypes)
                .FirstOrDefaultAsync(v => v.VenueId == id.Value);
            if (venue == null) return NotFound();

            return View(venue);
        }

        // POST: Venues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venue = await _db.Venues.FindAsync(id);
            if (venue != null)
            {
                _db.Venues.Remove(venue);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}