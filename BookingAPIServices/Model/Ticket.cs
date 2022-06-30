using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BookingAPIServices.Model;

namespace BookingApi.Model
{
    public class Ticket
    {
        [Key]
        public int TicketId { get; set; }

        [ForeignKey("Inventory")]
        public int InventoryId { get; set; }

        public int ModifiedId { get; set; }

        public DateTime ModifiedDate { get; set; }
        public string TicketPNR { get; set; }

        public string EmailId { get; set; }

        public string DiscountCode { get; set; }

        public string Rountrip { get; set; }
        public string OptMeal { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int NoofSeatedBooked { get; set; }
        public string SeatNumbers { get; set; }
        public int cost { get; set; }
        public string Type { get; set; }
        public bool IsActive { get; set; }

        public virtual Inventory Inventory { get; set; }

        //public virutal Discount Discount {get;set;}


    }
}

