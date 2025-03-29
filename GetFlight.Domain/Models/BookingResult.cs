using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetFlight.Domain.Models
{
    public class BookingResult
    {
        public bool Success { get; set; }
        public string BookingReference { get; set; }
        public string Message { get; set; }
    }
}
