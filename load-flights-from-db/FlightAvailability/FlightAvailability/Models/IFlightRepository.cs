using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightAvailability.Models
{
    public interface IFlightRepository
    {
        Task<List<Flight>> findByOriginAndDestination(string origin, string destination);
    }
}
