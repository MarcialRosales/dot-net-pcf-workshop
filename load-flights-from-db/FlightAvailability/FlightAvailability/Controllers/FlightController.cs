using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Web.Http;
using FlightAvailability.Models;
using System;
using NLog;

namespace FlightAvailability.Controllers
{

    [Route("api")]
    public class FlightController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private IFlightRepository _flightService;

        public FlightController(IFlightRepository flightService)
        {
            this._flightService = flightService;
            logger.Debug("Created FlightController ");

        }

        [HttpGet]
        public async Task<List<Flight>> find([FromUri, Required] string origin, [FromUri, Required] string destination)
        {
            logger.Debug($"Find {origin}/{destination}");
            return await _flightService.findByOriginAndDestination(origin, destination);


        }
    }


}