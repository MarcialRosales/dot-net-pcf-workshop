namespace FlightAvailability.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    using FlightAvailability.Models;
    using System.Collections.Generic;

    internal sealed class Configuration : DbMigrationsConfiguration<FlightAvailability.Models.FlightContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(FlightAvailability.Models.FlightContext context)
        {

            if (!context.Flight.Any())
            {
                InitialFlights().ForEach(f => context.Flight.Add(f));

            }

        }
        private List<Flight> InitialFlights()
        {
            var flights = new List<Flight>
            {
                new Flight{Origin="MAD",Destination="GTW",Date="18Apr2017"},
                new Flight{Origin="MAD",Destination="FRA",Date="18Apr2017"},
                new Flight{Origin="MAD",Destination="LHR",Date="18Apr2017"},
                new Flight{Origin="MAD",Destination="ACE",Date="18Apr2017"},
                new Flight{Origin="MAD",Destination="GTW",Date="19Apr2017"},
                new Flight{Origin="MAD",Destination="FRA",Date="19Apr2017"},
                new Flight{Origin="MAD",Destination="LHR",Date="19Apr2017"},
                new Flight{Origin="MAD",Destination="ACE",Date="19Apr2017"}
            };
            return flights;
        }
    }
}
