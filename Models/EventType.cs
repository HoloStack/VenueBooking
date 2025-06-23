using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace venueBooking.Models
{
    public class EventType
    {
        public int EventTypeId { get; set; }

        [Required]
        public string Name { get; set; }

        // Venues that support this event type
        public ICollection<Venue>? Venues { get; set; }

        // Events of this type
        public ICollection<Event>? Events { get; set; }
    }
}