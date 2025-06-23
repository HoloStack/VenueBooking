using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace venueBooking.Models
{
    public class Venue
    {
        public int VenueId { get; set; }

        [Required]
        public string VenueName { get; set; }

        public string Location { get; set; }

        public int Capacity { get; set; }

        // Nullable so it isn’t required when creating
        public string? ImageUrl { get; set; }

        // Exclude from model binding/validation
        [BindNever]
        public ICollection<Booking>? Bookings { get; set; }
    }
}