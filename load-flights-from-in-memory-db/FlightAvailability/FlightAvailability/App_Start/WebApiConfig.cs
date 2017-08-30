using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
 
using System.Web.Http.Dependencies;

using FlightAvailability.Models;
using Microsoft.Practices.Unity;

namespace FlightAvailability
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            InitializeDataStore();

            InitializeDI(config);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
        
        private static void InitializeDataStore()
        {
            System.Data.Entity.Database.SetInitializer(new System.Data.Entity.MigrateDatabaseToLatestVersion<FlightAvailability.Models.FlightContext, 
                FlightAvailability.Migrations.Configuration>());

            var configuration = new FlightAvailability.Migrations.Configuration();
            var migrator = new System.Data.Entity.Migrations.DbMigrator(configuration);
            if (migrator.GetPendingMigrations().Any())
            {
                System.Diagnostics.Debug.WriteLine("Running migrations against database");
                migrator.Update();
            }else
            {
                System.Diagnostics.Debug.WriteLine("There no database schema changes");
            }
        }

        private static void InitializeDI(HttpConfiguration config)
        {
            var container = new UnityContainer();
           
            container.RegisterType<IFlightRepository, FlightRepository>();

            // Configure .Net MVC to use UnitContainer to resolve dependencies when creating Controller classes
            config.DependencyResolver = new UnityResolver(container);
        }
    }
    public class UnityResolver : IDependencyResolver
    {
        protected IUnityContainer container;

        public UnityResolver(IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            this.container = container;
        }

        public object GetService(Type serviceType)
        {
            System.Diagnostics.Debug.WriteLine($"GetService {serviceType}");
            try
            {
                return container.Resolve(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return container.ResolveAll(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return new List<object>();
            }
        }

        public IDependencyScope BeginScope()
        {
            System.Diagnostics.Debug.WriteLine("BeginScope ");
            var child = container.CreateChildContainer();
            return new UnityResolver(child);
        }

        public void Dispose()
        {
            System.Diagnostics.Debug.WriteLine("Dispose ");
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            container.Dispose();
        }
    }


}
