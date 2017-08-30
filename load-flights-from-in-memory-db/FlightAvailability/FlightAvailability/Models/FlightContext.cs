using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace FlightAvailability.Models
{
    public class FlightContext : DbContext
    {

        public FlightContext() :
            base("flightContext4")
        {

        }
        public DbSet<Flight> Flight { get; set; }
    }
}