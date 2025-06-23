using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace venueBooking.Models
{
    public class EventType
    {
        public int EventTypeId { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<Event> Events { get; set; }
        public ICollection<Venue> Venues { get; set; }
    }
}