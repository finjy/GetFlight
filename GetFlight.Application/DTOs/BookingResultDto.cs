using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetFlight.Application.DTOs
{
    public class BookingResultDto
    {
        public bool Success { get; set; }
        public string BookingReference { get; set; }
        public string Message { get; set; }
    }
}
