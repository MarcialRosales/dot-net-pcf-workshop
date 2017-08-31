
using System;
using System.ComponentModel.DataAnnotations;

namespace FlightAvailability.Models
{
    public class Flight
    {
        public long Id { get; set; }

        [StringLength(10)]
        public string Origin { get; set; }
        [StringLength(10)]
        public string Destination { get; set; }
        [StringLength(250)]
        public string Date { get; set;  }
        [StringLength(25)]
        public string Code { get; set;  }

        public override string ToString()
        {
            return $"Flight[{Id} {Origin}/{Destination}]";
        }
    }

}