using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingAPIServices.Model
{
    public class Inventory
    {
        [Key]
        public int InventoryId { get; set; }

        [ForeignKey("AirlineLogo")]
        public int AirlineId { get; set; }
        
        public bool ISBlocked { get; set; }

        public DateTime BookedDate { get; set; }
        public bool IsActive { get; set; }
        public int No_Booked_BC { get; set; }
        public int No_Booked_NonBC { get; set; }
        public string FromPlace { get; set; }
        public string ToPlace { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int ModifieduserId { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int Total_BusinessClass_Seat { get; set; }
        public int Total_NonBusinessClass_Seat { get; set; }
        public string InstrumentUsed { get; set; }
        public int TicketCost { get; set; }

        public virtual AirlineLogo AirlineLogo { get; set; }
       

    }
}
