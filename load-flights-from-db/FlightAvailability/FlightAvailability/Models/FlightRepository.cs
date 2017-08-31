using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using NLog;

namespace FlightAvailability.Models
{
    public class FlightRepository : IFlightRepository
    {
        private FlightContext _ctx;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        /*
        public FlightRepository(FlightContext ctx)
        {
            _ctx = ctx;
            logger.Debug("Created FlightRepository");
        }
        */
        public FlightRepository()
        {
            logger.Debug("Created FlightRepository");
        }

        public async Task<List<Flight>> findByOriginAndDestination(string origin, string destination)
        {
            logger.Debug("findByOriginAndDestination");
            using (_ctx = new FlightContext())
            {
                logger.Debug("findByOriginAndDestination using context");
                return await _ctx.Flight
                .Where(f => f.Origin == origin && f.Destination == destination).ToListAsync();
            }
            
          
        }
    }
}