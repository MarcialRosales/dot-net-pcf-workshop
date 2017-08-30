using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;


namespace FlightAvailability.Models
{
    public class FlightRepository : IFlightRepository
    {
        private FlightContext _ctx;
      

        public FlightRepository(FlightContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<List<Flight>> findByOriginAndDestination(string origin, string destination)
        {
            return await _ctx.Flight
                .Where(f => f.Origin == origin && f.Destination == destination).ToListAsync();
          
        }
    }
}