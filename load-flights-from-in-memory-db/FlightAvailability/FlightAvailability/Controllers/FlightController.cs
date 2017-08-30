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

        public FlightController()
        {
        }

        [HttpGet]
        public async Task<List<Flight>> find([FromUri, Required] string origin, [FromUri, Required] string destination)
        {
            System.Diagnostics.Debug.WriteLine($"Find {origin}/{destination}");
            return InitialFlights();


        }
        private List<Flight> InitialFlights()
        {
            var flights = new List<Flight>
            {
                new Flight{Origin="MAD",Destination="GTW"},
                new Flight{Origin="MAD",Destination="FRA"},
                new Flight{Origin="MAD",Destination="LHR"},
                new Flight{Origin="MAD",Destination="ACE"}
            };
            return flights;
        }
    }


}