using System;
using System.ComponentModel.DataAnnotations;

namespace venueBooking.Models
{
    public class Booking
    {
        public int BookingId { get; set; }

        [Required]
        public DateTime BookingDate { get; set; }
        
        [Required]
        public string CustomerName { get; set; }

        [Required]
        public int EventId { get; set; }
        public Event Event { get; set; }

        [Required]
        public int VenueId { get; set; }
        public Venue Venue { get; set; }
    }
}