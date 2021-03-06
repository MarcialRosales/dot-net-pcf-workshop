PCF ASP.NET Developers workshop
==

<!-- TOC depthFrom:1 depthTo:6 withLinks:1 updateOnSave:1 orderedList:0 -->

- [Introduction](#Introduction)
- [Pivotal Cloud Foundry Technical Overview](#pivotal-cloud-foundry-technical-overview)
- [Deploying simple apps](#deploying-simple-apps)
  - [Lab - Deploy ASP.NET app](#deploy-dot-net-app)
  - [Lab - Deploy web site](#deploy-web-site)
  - [Quick Introduction to Buildpacks](#quick-intro-buildpack)
  - [Deploying applications with application manifest](#deploying-applications-with-application-manifest)
  - [Platform guarantees](#Platform-guarantees)
- [Cloud Foundry services](#cloud-foundry-services)
  - [Lab - Load flights from a database](#load-flights-from-a-provisioned-database)  


<!-- /TOC -->
# Introduction

## Prerequisites

- .Net 4.5 SDK
- Latest git client and ideally your own github account but not essential
- `curl` or `Postman` (https://www.getpostman.com/) or similar http client. Otherwise you can use the browser, ideally with some plugin to render JSON.

### Login to Cloud Foundry's Apps Manager

You must have received an email with the credentials (username, password) to access Cloud Foundry. Go to the URL provided by the instructor. Upon a successful login, you are entering a web application called **Apps Manager**.

### Install Cloud Foundry Command-Line Interface (cli)

To install the CLI you can either go to the **Tools** option in the right hand side menu of the **Apps Manager** and follow the instructions in the page. Or you can download it from https://docs.cloudfoundry.org/cf-cli/install-go-cli.html.

### Login to Cloud Foundry thru CLI

Go to the **Tools** option in the **Apps Manager** and copy the `cf login` command and execute from a Terminal. Enter your username and password.
You are probably prompted to select one organization. Select the organization that matches your name.

At this point, you are logged into PCF and are ready to push applications to your targeted environment.

> Targeted environment: PCF is organized into several organizations and each organization into one or many spaces. We must select the organization and space where we want to deploy apps.  

To know the environment you are currently targeting or

# Pivotal Cloud Foundry Technical Overview

Reference documentation:
- https://docs.pivotal.io
- [Elastic Runtime concepts](http://docs.pivotal.io/pivotalcf/concepts/index.html)


# Deploying simple apps

CloudFoundry excels at the developer experience: deploy, update and scale applications on-demand regardless of the application stack (java, php, node.js, go, etc).  We are going to learn how to deploy 4 types of applications: .Net app and static web pages.

Reference documentation:
- [Using Apps Manager](http://docs.pivotal.io/pivotalcf/1-9/console/index.html)
- [Using cf CLI](http://docs.pivotal.io/pivotalcf/1-9/cf-cli/index.html)
- [Deploying Applications](http://docs.pivotal.io/pivotalcf/1-9/devguide/deploy-apps/deploy-app.html)
- [Deploying with manifests](http://docs.pivotal.io/pivotalcf/1-9/devguide/deploy-apps/manifest.html)

In the next sections we are going to deploy a ASP.NET  MVC application and a web site. Before we proceed with the next sections we are going to checkout the following repository which has the .Net projects we are about to deploy.

1. `git clone https://github.com/MarcialRosales/dot-net-pcf-workshops.git`
2. `cd dot-net-pcf-workshops`

## <a name="lab1"></a> Lab 1 - Deploy a ASP.NET MVC app
We have created an application was created using the out of the box ASP.NET MVC template with Web API. We have customized the html layout of the generated app.
This application is under `skeleton/FlightAvailability` folder.

We open the project in Visual Studio and run it locally to see that it works.

We are now ready to deploy this simple application to PCF. We follow these steps:
1. Ensure project is built (on Windows): > Build menu > Publish option > Select publish to folder
2. Go to the publish folder
3. Deploy app to PCF:

  `cf push FlightAvailability -s windows2012R2 -b hwc_buildpack`

Once the application has successfully deployed, PCF shows us in the console the following information:
```
requested state: started
instances: 1/1
usage: 1G x 1 instances
urls: dotnet-flightavailability.cfapps.pez.pivotal.io
last uploaded: Wed Aug 30 09:08:11 UTC 2017
stack: windows2012R2
buildpack: hwc_buildpack

state     since                         cpu    memory        disk          details
#0   running   2017-08-30 11:08:38 AM   0.0%   43.6M of 1G   24.8M of 1G
```

Lets go to the browser and check our application. Which is the URL?


> NOTE: If hwc_buildpack is not installed, point the buildpack directly to the gitHub location. For Windows cells: must point to a zip file for now.

  `cf push FlightAvailability-s windows2012R2  -b https://github.com/cloudfoundry-incubator/hwc-buildpack/releases/download/v2.3.3/hwc_buildpack-cached-v2.3.3.zip`

> NOTE: (From GSS playbook) PCF 1.10: .NET apps must use the HWC buildpack or a custom buildpack that offers the same functionality; standalone/executable apps must use the binary buildpack with a start command


### About Web.Config

Most application Web.configs work out of the box with PCF, however here are a couple of things to watch out for.

- Don’t use Windows integrated auth, it’s been disabled in PCF.
- Don’t use HTTP modules that don’t ship with .NET or can’t be deployed in your app’s bin directory, for example the [Micorsoft URL Rewrite module](https://www.iis.net/downloads/microsoft/url-rewrite).
- SQL Server connection strings must use fully qualified domain names.


## <a name="lab2"></a> Lab 2 - Retrieve flights thru a REST endpoint

We create a new project `load-flights-from-in-memory-db/FlightAvailability` starting with the content of our previous project: `skeleton/FlightAvailability`.

To add a REST endpoint:
1. Add a `FlightController` class that returns a collection of dummy `Flight`(s) to the `Controllers` namespace.
2. Add a `Flight` class to the `Models` namespace.

To deploy it:
1. **Publish** to a folder using Visual Studio
2. Go to the publish folder
3. Run
  `cf push dotnet-FlightAvailability -s windows2012R2 -b hwc_buildpack`
4. Test it with `curl` or *Postman*:
  `curl 'dotnet-flightavailability.cfapps.pez.pivotal.io/api?origin=MAD&destination=FRA' | jq .`


### Add global error handler

Our application should have a global error (added to `Global.asax.cs` file) handler otherwise should the application failed to start we would not know why.

```
void Application_Error(object sender, EventArgs e)
 {
     Exception lastError = Server.GetLastError();
     Console.WriteLine("Unhandled exception: " + lastError.Message + lastError.StackTrace);
 }
```

## <a name="lab3"></a> Lab 3 - Deploy a static web site
Deploy static site associated to the flight availability and make it internally available on a given private domain.
The static site corresponds to the API documentation of our flight-availability application. It is found under `load-flights-from-in-memory-db/flight-availability-doc` folder.  

> Note: We generated this api doc using [ReDoc](https://github.com/Rebilly/ReDoc) from a Swagger definition that we have downloaded from the .Net core version of our flight-availability found in another [repo](https://github.com/MarcialRosales/dot-net-core-pcf-workshop/tree/master/load-flights-from-in-memory-db/flight-availability-api-doc). Check this [section](https://github.com/MarcialRosales/dot-net-core-pcf-workshop#-deploy-a-web-site) to know how it was generated.

1. Assuming you are under `load-flights-from-in-memory-db`
2. Deploy the app  
  `cf push flight-availability-api -p FlightAvailability-Doc --random-route`  
3. Check out application's details, whats the url?  
  `cf app flight-availability-api`  
4. How did PCF know that this was a static site and not a .Net application?


## <a name="quick-intro-buildpack"></a> Quick Introduction to Buildpacks

We have pushed two applications, a .Net Core and a static web site. We know that for the .Net Core we need a .Net Core Runtime and to run the static web site we need a web server like Apache or Nginx.

From [.Net buildpack](https://docs.cloudfoundry.org/buildpacks/dotnet-core/index.html#pushing-apps) ...
> Cloud Foundry automatically uses the .NET Core buildpack when one or more of the following conditions are met:

>- The pushed app contains one or more &ast;.csproj or &ast;.fsproj files.
>- The pushed app contains one or more project.json files.
>- The app is pushed from the output directory of the dotnet publish command.

> If your app requires external shared libraries that are not provided by the rootfs or the buildpack, you must place the libraries in an ld_library_path directory at the app root.

From [Static buildpack](https://docs.cloudfoundry.org/buildpacks/staticfile/#staticfile) ...
> Cloud Foundry requires a file named Staticfile in the root directory of the app to use the Staticfile buildpack with the app.


## Deploying applications with application manifest

Rather than passing a potentially long list of parameters to `cf push` we are going to move those parameters to a file so that we don't need to type them everytime we want to push an application. This file is called  *Application Manifest*.

The equivalent *manifest* file for the command `cf push flight-availability -p publish -i 2 --hostname fa` is:

```
---
applications:
- name: flight-availability
  instances: 2
  path: publish
  host: fa
```


*Things we can do with the manifest.yml file* (more details [here](http://docs.pivotal.io/pivotalcf/1-9/devguide/deploy-apps/manifest.html))
- [ ] simplify push command with manifest files (`-f <manifest>`, `-no-manifest`)
- [ ] register applications with DNS (`domain`, `domains`, `host`, `hosts`, `no-hostname`, `random-route`, `routes`). We can register http and tcp endpoints.
- [ ] deploy applications without registering with DNS (`no-route`) (for instance, a messaging based server which does not listen on any port)
- [ ] specify compute resources : memory size, disk size and number of instances!! (Use manifest to store the 'default' number of instances ) (`instances`, `disk_quota`, `memory`)
- [ ] specify environment variables the application needs (`env`)
- [ ] as far as CloudFoundry is concerned, it is important that application start (and shutdown) quickly. If we are application is too slow we can adjust the timeouts CloudFoundry uses before it deems an application as failed and it restarts it:
	- `timeout` (60sec) Time (in seconds) allowed to elapse between starting up an app and the first healthy response from the app
	- `env: CF_STAGING_TIMEOUT` (15min) Max wait time for buildpack staging, in minutes
	- `env: CF_STARTUP_TIMEOUT` (5min) Max wait time for app instance startup, in minutes
- [ ] CloudFoundry is able to determine the health status of an application and restart if it is not healthy. We can tell it not to check or to checking the port (80) is opened or whether the http endpoint returns a `200 OK` (`health-check-http-endpoint`, `health-check-type`)
- [ ] CloudFoundry builds images from our applications. It uses a set of scripts to build images called buildpacks. There are buildpacks for different type of applications. CloudFoundry will automatically detect the type of application however we can tell CloudFoundry which buildpack we want to use. (`buildpack`)
- [ ] specify services the application needs (`services`)

## Platform guarantees

We have seen how we can scale our application (`cf scale -i #` or `cf push  ... -i #`). When we specify the number of instances, we create implicitly creating a contract with the platform. The platform will try its best to guarantee that the application has those instances. Ultimately the platform depends on the underlying infrastructure to provision new instances should some of them failed. If the infrastructure is not ready available, the platform wont be able to comply with the contract. Besides this edge case, the platform takes care of our application availability.

Let's try to simulate our application crashed. To do so go to the home page and click on the link `KillApp`.

If we have +1 instances, we have zero-downtime because the other instances are available to receive requests while PCF creates a new one. If we had just one instance, we have downtime of a few seconds until PCF provisions another instance.


## <a name="deploy-dot-net-app"></a> Lab 4 - Retrieve flights thru a REST endpoint from a in-memory db

We are going to load the `Flight` entity using Entity Framework 6 and we are going to manage the schema using *Migrations*.

1. Add Entity Framework
  `Install-Package EntityFramework`
2. Add `FlightContext`, `FlightRepository` and `IFlightRepository` classes
3. We want to manage the schema thru *Migrations* therefore we are going to enable it. From the *Package Manager Console* we run the following command. It produces a new folder `Migrations` with a `Configuration` class with a `Seed` method that we use to populate the schema with data.
  `Enable-Migrations`
4. We add the logic that automatically creates the database schema and applies schema upgrades. We add it to the class `WebApiConfig.cs` although it should be on a dedicated class. We call this method from the `Register` method.
  if there are schema changes to apply, we should see in the logs `Running migrations against database` else `There no database schema changes`.

  ```
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
    ```
5. We instruct *Migrations* to generate the *code* that produces the schema corresponding to the current model. *Migrations* will produce a class called `InitialFlightTable` to the `Migrations` namespace.
  `add-migration InitialFlightTable`

6. We populate the table with some data thru the `Configuration.cs` class.
  ```
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
  ```

We could run the application as it stands now. It would create the schema but that's it because the controller is still returning dummy data.

We need to modify it so that it uses an instance of `IFlightRepository`. We are going to use *Unity* DI container to register services like `FlightRepository` and others and inject them to the `Controller` classes.

1. We add the package to use *Unity* DI container
  `Install-Package Unity.WebAPI`
2. We modify the `FlightController` so that we inject a `IFlightRepository` thru the constructor and use it to find flights.
  ```
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
 ```
3. Now, we need to set up the DI container in the `Global.asax.cs` class.
  ```
  protected void Application_Start()
       {
           AreaRegistration.RegisterAllAreas();
           UnityConfig.RegisterComponents();

           ...
       }
  ```
4. We need to register our classes in UnityConfig.cs class
  ```

  ```

We are now ready to test it. Run it locally from Visual Studio and run visit the following url in the browser: `http://localhost:52025/api?origin=MAD&destination=FRA`.  We should get back 2 fights.


## <a name="deploy-dot-net-app"></a> Lab 4 - Retrieve flights thru a REST endpoint from a SQL database

1. Add MySQL driver (https://dev.mysql.com/doc/connector-net/en/connector-net-entityframework60.html)
  `Install-Package MySql.Data.Entity`
2. 

### Add logging

1. `Install-Package NLog.Config`

### Access Mysql database managed by PCF

There are 2 options:
1) SSH tunneling + use mysql client: https://docs.pivotal.io/pivotalcf/1-11/devguide/deploy-apps/ssh-services.html
2) Push a SQL mgmt app like this one: https://docs.pivotal.io/p-mysql/2-0/use.html#dev-tools

We are going to use option 1).
