using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FlightAvailability.Controllers
{
    [Route("mgt")]
    public class ActuatorController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public ActuatorController()
        {
            logger.Debug("Created ActuatorController");

        }
        [HttpGet]
        public AppInfo Info()
        {
            logger.Debug("ActuatorController.Info");
            return new AppInfo("FlightAvailability");
        }
    }
    public class AppInfo
    {
        public string name { get; set; }

        public AppInfo(string name)
        {
            this.name = name;
        }
    }
}
