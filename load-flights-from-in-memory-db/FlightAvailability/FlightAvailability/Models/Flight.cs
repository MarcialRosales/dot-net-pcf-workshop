
using System;

namespace FlightAvailability.Models
{
    public class Flight
    {
        public long Id { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string Date { get; set;  }
        public string Code { get; set;  }

        public override string ToString()
        {
            return $"Flight[{Id} {Origin}/{Destination}]";
        }
    }

}