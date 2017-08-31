using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using MySql.Data.Entity;
using System.Data.Common;

namespace FlightAvailability.Models
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class FlightContext : DbContext
    {

        public FlightContext() :
            base()
        {

        }
        public FlightContext(DbConnection existingConnection, bool contextOwnsConnection) :
          base(existingConnection, contextOwnsConnection)
        {

        }
        public DbSet<Flight> Flight { get; set; }
    }
}