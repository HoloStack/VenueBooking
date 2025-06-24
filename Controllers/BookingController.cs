using venueBooking.Data;
using venueBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
        public async Task<IActionResult> Index(int? venueId, int? eventId, DateTime? fromDate, DateTime? toDate)
        {
            var query = _db.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .AsQueryable();

            // Apply filters
            if (venueId.HasValue)
                query = query.Where(b => b.VenueId == venueId.Value);
                
            if (eventId.HasValue)
                query = query.Where(b => b.EventId == eventId.Value);
                
            if (fromDate.HasValue)
                query = query.Where(b => b.BookingDate >= fromDate.Value.Date);
                
            if (toDate.HasValue)
                query = query.Where(b => b.BookingDate <= toDate.Value.Date);

            var bookings = await query
                .OrderByDescending(b => b.BookingDate)
                .ThenBy(b => b.CustomerName)
                .ToListAsync();

            // Populate filter dropdowns
            ViewBag.Venues = new SelectList(await _db.Venues.OrderBy(v => v.VenueName).ToListAsync(), "VenueId", "VenueName", venueId);
            ViewBag.Events = new SelectList(await _db.Events.OrderBy(e => e.EventName).ToListAsync(), "EventId", "EventName", eventId);
            
            // Set filter values for form
            ViewBag.VenueId = venueId;
            ViewBag.EventId = eventId;
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

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
        public async Task<IActionResult> Create(int? eventId)
        {
            // Create a new booking model
            var booking = new Booking();
            
            // Load all events for initial dropdown
            var allEvents = await _db.Events
                .Include(e => e.Venue)
                .OrderBy(e => e.EventDate)
                .Select(e => new {
                    EventId = e.EventId,
                    DisplayText = e.EventName + " (" + e.EventDate.ToString("yyyy-MM-dd HH:mm") + " at " + e.Venue.VenueName + ")"
                })
                .ToListAsync();
            
            // If eventId is provided, pre-select it and populate related fields
            if (eventId.HasValue)
            {
                var selectedEvent = await _db.Events
                    .Include(e => e.Venue)
                    .FirstOrDefaultAsync(e => e.EventId == eventId.Value);
                    
                if (selectedEvent != null)
                {
                    booking.EventId = eventId.Value;
                    booking.VenueId = selectedEvent.VenueId;
                    booking.BookingDate = selectedEvent.EventDate.Date;
                    
                    ViewBag.PreSelectedEvent = selectedEvent.EventName;
                    ViewBag.PreSelectedVenue = selectedEvent.Venue?.VenueName;
                    ViewBag.PreSelectedEventDate = selectedEvent.EventDate.ToString("yyyy-MM-dd HH:mm");
                }
            }
                
            ViewBag.AllEvents = new SelectList(allEvents, "EventId", "DisplayText", eventId);
            ViewBag.Events = new SelectList(Enumerable.Empty<Event>(), "EventId", "EventName");
            ViewBag.Venues = new SelectList(await _db.Venues.ToListAsync(), "VenueId", "VenueName", booking.VenueId);
            return View(booking);
        }

        // API endpoint to get events for a specific venue
        [HttpGet]
        public async Task<IActionResult> GetEventsForVenue(int venueId)
        {
            var events = await _db.Events
                .Where(e => e.VenueId == venueId)
                .OrderBy(e => e.EventDate)
                .Select(e => new { 
                    Value = e.EventId, 
                    Text = e.EventName,
                    Date = e.EventDate.ToString("yyyy-MM-dd HH:mm"),
                    VenueId = e.VenueId
                })
                .ToListAsync();
            
            return Json(events);
        }

        // API endpoint to get event details
        [HttpGet]
        public async Task<IActionResult> GetEventDetails(int eventId)
        {
            var eventDetails = await _db.Events
                .Where(e => e.EventId == eventId)
                .Select(e => new {
                    EventId = e.EventId,
                    EventName = e.EventName,
                    VenueId = e.VenueId,
                    VenueName = e.Venue.VenueName,
                    EventDate = e.EventDate.ToString("yyyy-MM-dd HH:mm"),
                    DateOnly = e.EventDate.ToString("yyyy-MM-dd")
                })
                .FirstOrDefaultAsync();
            
            return Json(eventDetails);
        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            // Check venue capacity before allowing booking
            if (ModelState.IsValid)
            {
                var eventDetails = await _db.Events
                    .Include(e => e.Venue)
                    .FirstOrDefaultAsync(e => e.EventId == booking.EventId);
                
                if (eventDetails != null)
                {
                    var currentBookingCount = await _db.Bookings
                        .CountAsync(b => b.EventId == booking.EventId);
                    
                    if (currentBookingCount >= eventDetails.Venue.Capacity)
                    {
                        ModelState.AddModelError("", $"Sorry, this event is fully booked. The venue capacity is {eventDetails.Venue.Capacity} and there are already {currentBookingCount} bookings.");
                    }
                    else
                    {
                        _db.Add(booking);
                        await _db.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Selected event not found.");
                }
            }
            
            // Re-populate dropdowns on error
            var allEvents = await _db.Events
                .Include(e => e.Venue)
                .OrderBy(e => e.EventDate)
                .Select(e => new {
                    EventId = e.EventId,
                    DisplayText = e.EventName + " (" + e.EventDate.ToString("yyyy-MM-dd HH:mm") + " at " + e.Venue.VenueName + ")"
                })
                .ToListAsync();
                
            ViewBag.AllEvents = new SelectList(allEvents, "EventId", "DisplayText", booking.EventId);
            ViewBag.Events = new SelectList(Enumerable.Empty<Event>(), "EventId", "EventName", booking.EventId);
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