using System;
using System.ComponentModel.DataAnnotations;

namespace venueBooking.Models
{
    public class Event
    {
        public int EventId { get; set; }

        [Required]
        public string EventName { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        public DateTime EventDate { get; set; }

        public string Description { get; set; }

        [Required]
        public int VenueId { get; set; }
        public Venue Venue { get; set; }

        [Required]
        public int EventTypeId { get; set; }
        public EventType EventType { get; set; }
    }
}