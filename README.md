# JCTools.GenericCrud

Simplification of the **C**reate, **R**ead, **U**pdate and **D**elete web pages of the application models.

## Overview

All application required multiple pages for edited the base models. This pages generally are equals to each other.

This package allows reduce this task at minimum of actions.

You only require create and configure your models, and this package create the necessary controllers, views and actions for the **C**reate, **R**ead, **U**pdate and **D**elete actions.

## Status
![v2.0.0-beta4](https://img.shields.io/badge/nuget-v2.0.0%20beta4-blue)

## Requirements
![.net core 2.1](https://img.shields.io/badge/.net%20core-v2.1-green),
![.net core 3.1](https://img.shields.io/badge/.net%20core-v3.1-green) or 
![.net 5.0](https://img.shields.io/badge/.net-v5.0-green)

![bootstrap 3.3.7](https://img.shields.io/badge/bootstrap-v3.3.7-blue) or
![bootstrap 4.3.1](https://img.shields.io/badge/bootstrap-v4.3.1-blue)

![font awesome 5.0.6](https://img.shields.io/badge/font%20awesome-v5.0.6-blue)

## Usage

1. Add the package to your application
    ```bash
    Install-Package JCTools.GenericCrud -Version 2.0.0-beta4
    ```
    Or
    ```bash
    dotnet add package JCTools.GenericCrud --version 2.0.0-beta4
    ```
2. Add the next lines in the method **ConfigureServices** of your **Startup** class
    ```cs
        services.ConfigureGenericCrud<MyContext>(options =>
        {
            // options is an instance from JCTools.GenericCrud.Settings.IOptions
            // use this interface to custom the generated CRUDs globally
            // eg;

            // Indicate if desired use Modals
            options.UseModals = true;
            // Set the bootstrap version to be used (default v4.3.1)
            options.BootstrapVersion = Settings.Bootstrap.Version3;
            // add the models type to manage with the package
            options.Models.Add<Models.Country>(); 
            options.Models.Add<Models.Genre>(nameof(Models.Genre.Name));
            
            // add a model with a custom controller
            options.Models.Add<Models.Movie, int, MovieController, Data.Context>();
        });
    ```

    > **Note:** From the version 2.0.0 the next features was marked how to obsolete and will be removed in future versions:
    > - The method *o.Models.Add(Type modelType, string keyPropertyName = "Id", string controllerName = "")*.
    > - The *ContextCreator* option

3. Run to app and access at the url **http://localhost:5000/[ModelName]**, sample: **http://localhost:5000/Country**. In the browser you should see a similar page to :
 ![Sample index page](Mockups/sampleIndexPage.png)

## Custom controllers
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

    > **Note:** In the version 2.0.0 the **Settings** property of the controller has initialized in the *OnActionExecuting(ActionExecutingContext filterContext)* or *OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)* controller methods; **You should move their custom settings of the controller constructor to this methods.**

2. Add the related model in the method **ConfigureServices** of your **Startup** class using specifying the custom controller, eg;
    ```cs
        options.Models.Add<Models.Movie, int, MovieController, Data.Context>();
    ```

3. **(optional)** If you override the **OnActionExecuting(ActionExecutingContext filterContext)** or **OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)** controller methods, make sure to invoke the base methods for the correct initializations of the controller settings

    ```cs
        //...
        
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Call the initialization of the Settings property
            base.InitSettings(context);
            // Add your custom settings here, eg;
            Settings.UseModals = false;
            Settings.Subtitle = "All entities";
            ViewBag.OtherEntities = (DbContext as Data.Context).OtherEntities.ToList();

            base.OnActionExecuting(context);
        }
        
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

4. Run to app and access at the url **http://localhost:5000/Movie**.

## Links and HTML anchors
To insert a link to a custom CRUD or CRUD, you only need to use ASP.NET Core Anchor Tag Helper.

```html
    <a asp-area="" asp-controller="MyEntity" asp-action="Index">My Label</a>
```
Notice that it was used in the entity model name instead of the controller name.

> **Note:** In .Net Core 2.1 the controller is named **Generic** and is required add the asp-route-entitySettings with the entity model name, eg;
    ```
        <a asp-area="" asp-controller="Generic" asp-action="Index" asp-route-entitySettings="MyEntity">My Label</a>
    ```



## Globalization and localization
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

## Changes of the version 2.0.0
* Add .net 5.0 support
* Add .net core 2.1 y 3.1 support
* Add support to Bootstrap 4.0
* Replaces the GenericController<TContext, TModel, TKey> class for the GenericController class
  * This new class is easier to use
* The follows interfaces was replaced for a best definition and structure:
  * IBase -> IViewModel
  * IBaseDetails, ICrudDetails -> IDetailsModel
  * ICrudEdit -> IEditModel
  * ICrudList -> IIndexModel
* The follows models was replaced by the **CrudModel** class
  * Base
  * CrudDetails
  * CrudEdit
  * CrudList
* The **IControllerOptions** interface and **ControllerOptions** class was removed for being unnecessary in the new structure
* The extensors methods **GetLocalizedString(...)** for the **IStringLocalizer** interfaces was moved to the **StringLocalizerExtensors** class
* The globalization and internationalization process is improved
* Now no need to use endpoint mapping, if you use older version remove the next code from the method **Configure** your **startup** class:
```cs
    app.UseMvc(routes =>
    {
        routes.MapCrudRoutes(); // remove this line

        // ...
    });   

```
## License
[MIT License](LICENSE)
