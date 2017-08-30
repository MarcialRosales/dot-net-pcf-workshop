using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Web.Http;
using FlightAvailability.Models;
using System;

namespace FlightAvailability.Controllers
{

    [Route("api")]
    public class FlightController : ApiController
    {

        private IFlightRepository _flightService;

        public FlightController(IFlightRepository flightService)
        {
            this._flightService = flightService;
        }

        [HttpGet]
        public async Task<List<Flight>> find([FromUri, Required] string origin, [FromUri, Required] string destination)
        {
            System.Diagnostics.Debug.WriteLine($"Find {origin}/{destination}");
            return await _flightService.findByOriginAndDestination(origin, destination);


        }
    }


}