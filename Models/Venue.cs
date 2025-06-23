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

        public string? ImageUrl { get; set; }

        // Navigation: events scheduled at this venue
        public ICollection<Event>? Events { get; set; }

        // Supported event types (many-to-many)
        [Display(Name = "Supported Event Types")]
        public ICollection<EventType>? SupportedEventTypes { get; set; }
    }
}