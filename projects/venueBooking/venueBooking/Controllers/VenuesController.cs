using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using venueBooking.Data;
using venueBooking.Models;
using venueBooking.Services;
using System.Threading.Tasks;
using System;

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

        // GET: Venues/Index
        public async Task<IActionResult> Index()
        {
            var venues = await _db.Venues.ToListAsync();
            return View(venues);
        }

        // GET: Venues/Create
        public IActionResult Create() => View();

        // POST: Venues/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Venue venue, IFormFile image)
        {
            if (!ModelState.IsValid)
                return View(venue);

            if (image != null && image.Length > 0)
            {
                venue.ImageUrl = await _blobService.UploadAsync(image);
            }

            _db.Venues.Add(venue);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}