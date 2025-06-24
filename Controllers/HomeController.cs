using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using venueBooking.Data;

namespace venueBooking.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /
        // GET: /Home
        public IActionResult Index()
        {
            return View(); // will render Views/Home/Index.cshtml
        }

        // Dashboard API endpoints
        [Route("api/dashboard/venues")]
        [HttpGet]
        public async Task<IActionResult> GetVenuesStats()
        {
            try
            {
                var venues = await _db.Venues.ToListAsync();
                var totalVenues = venues.Count;
                var totalCapacity = venues.Sum(v => v.Capacity);
                
                // Calculate available venues (venues without events today)
                var today = DateTime.Today;
                var venuesWithEventsToday = await _db.Events
                    .Where(e => e.EventDate.Date == today)
                    .Select(e => e.VenueId)
                    .Distinct()
                    .CountAsync();
                var availableVenues = totalVenues - venuesWithEventsToday;

                return Json(new {
                    total = totalVenues,
                    totalCapacity = totalCapacity,
                    available = Math.Max(0, availableVenues)
                });
            }
            catch
            {
                return Json(new { total = 0, totalCapacity = 0, available = 0 });
            }
        }

        [Route("api/dashboard/events")]
        [HttpGet]
        public async Task<IActionResult> GetEventsStats()
        {
            try
            {
                var now = DateTime.Now;
                var today = DateTime.Today;
                var nextWeek = today.AddDays(7);
                var next30Days = today.AddDays(30);

                var totalEvents = await _db.Events.CountAsync();
                var upcomingEvents = await _db.Events
                    .CountAsync(e => e.EventDate >= now && e.EventDate <= next30Days);
                var thisWeekEvents = await _db.Events
                    .CountAsync(e => e.EventDate >= today && e.EventDate < nextWeek);
                var nextWeekEvents = await _db.Events
                    .CountAsync(e => e.EventDate >= nextWeek && e.EventDate < nextWeek.AddDays(7));

                return Json(new {
                    total = totalEvents,
                    upcoming = upcomingEvents,
                    thisWeek = thisWeekEvents,
                    nextWeek = nextWeekEvents
                });
            }
            catch
            {
                return Json(new { total = 0, upcoming = 0, thisWeek = 0, nextWeek = 0 });
            }
        }

        [Route("api/dashboard/bookings")]
        [HttpGet]
        public async Task<IActionResult> GetBookingsStats()
        {
            try
            {
                var today = DateTime.Today;
                var nextWeek = today.AddDays(7);

                var totalBookings = await _db.Bookings.CountAsync();
                var todayBookings = await _db.Bookings
                    .CountAsync(b => b.BookingDate.Date == today);
                var thisWeekBookings = await _db.Bookings
                    .CountAsync(b => b.BookingDate >= today && b.BookingDate < nextWeek);

                return Json(new {
                    total = totalBookings,
                    today = todayBookings,
                    thisWeek = thisWeekBookings
                });
            }
            catch
            {
                return Json(new { total = 0, today = 0, thisWeek = 0 });
            }
        }

        // (optional) error page
        public IActionResult Error()
        {
            return View(); // render Views/Home/Error.cshtml
        }
    }
}
