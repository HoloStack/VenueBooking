using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace venueBooking.Models
{
    public class Event
    {
        public int EventId { get; set; }

        [Required]
        public string EventName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime EventDate { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public int VenueId { get; set; }
        public Venue? Venue { get; set; }

        [Required]
        public int EventTypeId { get; set; }
        public EventType? EventType { get; set; }

        public List<Booking>? Bookings { get; set; }
    }
}
