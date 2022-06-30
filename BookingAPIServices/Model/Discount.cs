using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookingAPIServices.Model
{
    public class Discount
    {
        [Key]
        public int DiscountId { get; set; }
        public string DiscountCode { get; set; }
        public string DiscountPercent { get; set; }
        public int ModifiedId { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
