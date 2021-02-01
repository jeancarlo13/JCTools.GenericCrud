# JCTools.GenericCrud

Simplification of the **C**reate, **R**ead, **U**pdate and **D**elete web pages of the application models.

## Content
- [JCTools.GenericCrud](#jctoolsgenericcrud)
  - [Content](#content)
  - [Overview](#overview)
  - [Status](#status)
  - [Requirements](#requirements)
  - [Usage](#usage)
    - [Basic usage](#basic-usage)
    - [Demo apps](#demo-apps)
  - [Other features](#other-features)
    - [Custom controllers](#custom-controllers)
    - [Entity model property settings](#entity-model-property-settings)
    - [Authorization](#authorization)
    - [Links and HTML anchors](#links-and-html-anchors)
    - [Globalization and localization](#globalization-and-localization)
  - [Release notes](#release-notes)
  - [License](#license)

## Overview

All application required multiple pages for edited the base models. This pages generally are equals to each other.

This package allows reduce this task at minimum of actions.

You only require create and configure your models, and this package create the necessary controllers, views and actions for the **C**reate, **R**ead, **U**pdate and **D**elete actions.


## Status
![v2.1.0](https://img.shields.io/badge/nuget-v2.1.0-blue)

## Requirements
![.net core 2.1](https://img.shields.io/badge/.net%20core-v2.1-green),
![.net core 3.1](https://img.shields.io/badge/.net%20core-v3.1-green) or 
![.net 5.0](https://img.shields.io/badge/.net-v5.0-green)

![bootstrap 3.3.7](https://img.shields.io/badge/bootstrap-v3.3.7-blue) or
![bootstrap 4.3.1](https://img.shields.io/badge/bootstrap-v4.3.1-blue)

![font awesome 5.0.6](https://img.shields.io/badge/font%20awesome-v5.0.6-blue)

## Usage
### Basic usage

1. Add the package to your application
    ```bash
    Install-Package JCTools.GenericCrud -Version 2.1.0
    ```
    Or
    ```bash
    dotnet add package JCTools.GenericCrud --version 2.1.0
    ```
2. Add the next lines in the method **ConfigureServices** of your **Startup** class
    ```cs
        services.ConfigureGenericCrud<MyContext>(options =>
        {
            // add the models type to manage with the package
            options.Models.Add<Models.Country>(); 
            options.Models.Add<Models.Genre>(nameof(Models.Genre.Name));
            // ...
            
            // options is an instance from JCTools.GenericCrud.Settings.IOptions
            // use this interface to custom the generated CRUDs globally
            // eg;

            // Indicate if desired use Modals
            options.UseModals = true;
            // Set the bootstrap version to be used (default v4.3.1)
            options.BootstrapVersion = Settings.Bootstrap.Version3;
            
            // add a model with a custom controller
            options.Models.Add<Models.Movie, int, MovieController, Data.Context>();            
        });
    ```
3. Run to app and access at the url **http://localhost:5000/[ModelName]**, sample: **http://localhost:5000/Country**. In the browser you should see a similar page to :
 ![Sample index page](Mockups/sampleIndexPage.png)
 ![Sample index page](Mockups/sampleIndexPage2.png)
    > Your app's layout page may make it look different from the images above.

### Demo apps
The current repository include 3 demo apps for showing the described features of the package:
- [.net Core 2.1 demo app](Test)
- [.net core 3.1 demo app](Test3.1)
- [.net 5.0 demo app](Test5.0)

## Other features
### Custom controllers
If your desired personalize your controllers, add additional actions or override the default actions, then

1. Create a new controller the inherits from **JCTools.GenericCrud.Controllers.GenericController**. e.g;
    ```cs
    using System;
    using System.Linq;
    using JCTools.GenericCrud.Controllers;
    using JCTools.GenericCrud.Services;
    using JCTools.GenericCrud.DataAnnotations;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;

    namespace MyApp.Controllers
    {
        [CrudConstraint(typeof(Models.MyModel))]
        public class MyController : GenericController
        {
            public MyController(
                IServiceProvider serviceProvider,
                IViewRenderService renderingService,
                IStringLocalizerFactory localizerFactory,
                ILoggerFactory loggerFactory
            )
                : base(serviceProvider, renderingService, localizerFactory, loggerFactory, "Id")
            {
                // add your custom logic here
            }
        }
    }
    ```

2. Add the related model in the method **ConfigureServices** of your **Startup** class using specifying the custom controller, eg;
    ```cs
        options.Models.Add<Models.Movie, int, MovieController, Data.Context>();
    ```

3. **(optional)** If you override the **OnActionExecuting(ActionExecutingContext filterContext)** or **OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)** controller methods, make sure to invoke the base methods for the correct initializations of the controller settings

    ```cs
        // ...
        
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Call the initialization of the Settings property
            base.InitSettings(context);
            
            // Add your custom settings here, eg;
            Settings.UseModals = false; // disabled the modals
            Settings.Subtitle = "All entities"; // change the default subtitle
            
            // Customizing the Icons and Buttons Classes of the Index Page
            var index = Settings as IIndexModel;
            index.NewAction.IconClass = "fa fa-plus-circle";
            index.NewAction.ButtonClass = "btn btn-success btn-sm";

            index.DetailsAction.IconClass = "fa fa-info";
            index.DetailsAction.ButtonClass = "btn btn-info btn-sm";
            
            index.EditAction.IconClass = "fa fa-edit";
            index.EditAction.ButtonClass = "btn btn-warning btn-sm";
            
            index.DeleteAction.IconClass = "fa fa-eraser";

            // other things
            ViewBag.Countries = (DbContext as Context).Countries.ToList();

            base.OnActionExecuting(context);
        }
        
        // Or
        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Call the initialization of the Settings property
            base.InitSettings(context);
            // Add your custom settings here, eg;
            Settings.UseModals = false;
            Settings.Subtitle = "All entities";
            ViewBag.OtherEntities = (DbContext as Data.Context).OtherEntities.ToList();

            return base.OnActionExecutionAsync(context, next);
        }
        //...
    ```

4. Run to app and access at the url **http://localhost:5000/[model name]**, e.g. http://localhost:5000/movie

> You can see a sample custom controller in the demo apps called MovieController:
> * [.net Core 2.1 demo app](Test/Controllers/MovieController.cs)
> * [.net core 3.1 demo app](Test3.1/Controllers/MovieController.cs) 
> * [.net 5.0 demo app](Test5.0/Controllers/MovieController.cs)

### Entity model property settings
Version 2.0.0 includes the ability to customize property settings in an entity model.
For this propose using the data annotation **CrudAttribute** in the namespace *JCTools.GenericCrud.DataAnnotations*.

This data annotation have third properties:

 * Visible: Boolean indicating that the property is or not visible in CRUD views.
 * UseCustomView: Boolean indicating that the property has custom views for rendering in details, create, delete, and edit actions.
    > Two custom views are required per property, one for read-only views (Details and Delete actions) and one for editable views (Create and Edit actions).
    > * Readonly views are named _Details<Property name>.cshtml.
    > * Editable views are named _Edit<Property name>.cshtml.    
    > eg; if the property is named *Status*, the CRUD expects to find two views named _DetailsStatus.cshtml and _EditStatus.cshtml.

 * IsEditableKey: Boolean that indicates whether the entity property is an Id/Key and whether or not it is editable by the user.
    > When an Id / Key property is editable, editing the entity is actually a deletion of the stored entity followed by the creation of a new entity using the new values.

### Authorization
JCTool.GenericCrud includes since version 2.1.0 the possibility of managing access to CRUD controllers using an authorization policy.

The name of the default policy is JCTools.GenericCrud.CrudPolicy, and by default authorization is not activated and anonymous access is allowed.

To turn on authorization validation, just add a call to **UseAuthorization** in your *Startup.ConfigureServices* method. e.g;

```cs
    services.ConfigureGenericCrud<MyContext>(options =>
    {
        // ...
        o.UseAuthorization(f => f.RequireAuthenticatedUser()); // add this line
    });
```
> **Note:** If no action is specified for policy validation, by default only one authenticated user is required.
### Links and HTML anchors
To insert a link to a custom CRUD or CRUD, you only need to use ASP.NET Core Anchor Tag Helper.

```html
    <a asp-area="" asp-controller="MyEntity" asp-action="Index">My Label</a>
```
Notice that it was used in the entity model name instead of the controller name.

> **Note:** In .Net Core 2.1 the controller is named **Generic** and is required add the asp-route-entitySettings attribute with the entity model name, eg;
    ```
        <a asp-area="" asp-controller="Generic" asp-action="Index" asp-route-entitySettings="MyEntity">My Label</a>
    ```

### Globalization and localization
By default, generic CRUDs support ASP.NET globalization and localization, as described in the official [documentation](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/localization?view=aspnetcore-5.0).

But only the english and spanish languages are included into the package.

You can extend or replace the included localized strings with your own translations by following the steps below:

1. Generate your own resources files (.rex), according the official documentation
2. Include the same keys that contains the include files [I18N.resx](JCTools.GenericCrud/Resources/I18N.resx) and [I18N.es.resx](JCTools.GenericCrud/Resources/I18N.es.resx)
3. Configure your **startup** class for use the globalization and localization middleware
4. Modify the initialization of the GenericCrud in the method **ConfigureServices** of your **Startup** class for include the next line:
   
   ```cs
    services.ConfigureGenericCrud<MyContext>(options =>
    {
        // ...
        o.ReplaceLocalization(Resources.MyResourcesClass.ResourceManager); // add this line
    });
    ```
    The *Resources.MyResourcesClass.ResourceManager* corresponds to the property *ResourceManager* of the autogenerated file in the step 1


## Release notes
In this [link](ReleaseNotes.md) you can be the release notes of the package.

## License
[MIT License](LICENSE)