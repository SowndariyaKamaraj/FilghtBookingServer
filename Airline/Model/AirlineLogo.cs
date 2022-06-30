using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineApiService.Model
{
    public class AirlineLogo
    {

        [Key]
        public int AirlineId { get; set; }
        public string AirlineName { get; set; }

        public long ContactNumber { get; set; }
        public string ContactAddress { get; set; }

        public string UploadLogo { get; set; }
        public int ModifiedId { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
