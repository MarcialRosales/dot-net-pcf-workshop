PCF .Net Core Developers workshop
==

<!-- TOC depthFrom:1 depthTo:6 withLinks:1 updateOnSave:1 orderedList:0 -->

- [Introduction](#Introduction)
- [Pivotal Cloud Foundry Technical Overview](#pivotal-cloud-foundry-technical-overview)
- [Deploying simple apps](#deploying-simple-apps)
  - [Lab - Deploy .Net Core app](#deploy-dot-net-app)
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

CloudFoundry excels at the developer experience: deploy, update and scale applications on-demand regardless of the application stack (java, php, node.js, go, etc).  We are going to learn how to deploy 4 types of applications: .Net Core app and static web pages.

Reference documentation:
- [Using Apps Manager](http://docs.pivotal.io/pivotalcf/1-9/console/index.html)
- [Using cf CLI](http://docs.pivotal.io/pivotalcf/1-9/cf-cli/index.html)
- [Deploying Applications](http://docs.pivotal.io/pivotalcf/1-9/devguide/deploy-apps/deploy-app.html)
- [Deploying with manifests](http://docs.pivotal.io/pivotalcf/1-9/devguide/deploy-apps/manifest.html)

In the next sections we are going to deploy a ASP.NET Core MVC application and a web site. Before we proceed with the next sections we are going to checkout the following repository which has the .Net projects we are about to deploy.

1. `git clone https://github.com/MarcialRosales/dot-net-pcf-workshops.git`
2. `cd dot-net-pcf-workshops`

## <a name="deploy-dot-net-app"></a> Deploy a ASP.NET MVC app
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


### Add global error handler

Before we deploy our application we are going to add a global error (added to `Global.asax.cs` file) handler otherwise should the application failed to start we would not know why.


### About Web.Config

Most application Web.configs work out of the box with PCF, however here are a couple of things to watch out for.

- Don’t use Windows integrated auth, it’s been disabled in PCF.
- Don’t use HTTP modules that don’t ship with .NET or can’t be deployed in your app’s bin directory, for example the [Micorsoft URL Rewrite module](https://www.iis.net/downloads/microsoft/url-rewrite).
- SQL Server connection strings must use fully qualified domain names.
