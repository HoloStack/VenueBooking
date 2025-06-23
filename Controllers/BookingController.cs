using venueBooking.Data;
using venueBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace venueBooking.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public BookingsController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            var bookings = await _db.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .ToListAsync();
            return View(bookings);
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _db.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null) return NotFound();
            return View(booking);
        }

        // GET: Bookings/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Events = new SelectList(await _db.Events.ToListAsync(), "EventId", "EventName");
            ViewBag.Venues = new SelectList(await _db.Venues.ToListAsync(), "VenueId", "VenueName");
            return View();
        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            if (ModelState.IsValid)
            {
                _db.Add(booking);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Events = new SelectList(await _db.Events.ToListAsync(), "EventId", "EventName", booking.EventId);
            ViewBag.Venues = new SelectList(await _db.Venues.ToListAsync(), "VenueId", "VenueName", booking.VenueId);
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _db.Bookings.FindAsync(id);
            if (booking == null) return NotFound();

            ViewBag.Events = new SelectList(await _db.Events.ToListAsync(), "EventId", "EventName", booking.EventId);
            ViewBag.Venues = new SelectList(await _db.Venues.ToListAsync(), "VenueId", "VenueName", booking.VenueId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Booking booking)
        {
            if (id != booking.BookingId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(booking);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _db.Bookings.AnyAsync(b => b.BookingId == id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Events = new SelectList(await _db.Events.ToListAsync(), "EventId", "EventName", booking.EventId);
            ViewBag.Venues = new SelectList(await _db.Venues.ToListAsync(), "VenueId", "VenueName", booking.VenueId);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _db.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null) return NotFound();
            return View(booking);
        }
    }
}