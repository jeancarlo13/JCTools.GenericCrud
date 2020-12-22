# JCTools.GenericCrud
![v2.0.0-beta1](https://img.shields.io/badge/nuget-v2.0.0%20beta1-blue)
![.net core 2.1](https://img.shields.io/badge/.net%20core-v2.1-green)

Simplification of the **C**reate, **R**ead, **U**pdate and **D**elete pages of the application models.

## Overview

All application required multiple pages for edited the base models. This pages generally are equals to each other.

This package allows reduce this task at minimum of actions.

You only require create and configure your models, and this package create the necessary controllers, views and actions for the **C**reate, **R**ead, **U**pdate and **D**elete actions.

## Usage

1. Add the package to your application
```bash
Install-Package JCTools.GenericCrud -Version 1.0.2

Or

dotnet add package JCTools.GenericCrud --version 1.0.2
```
2. Add the next lines in the method **ConfigureServices** of your **Startup** class
```cs
    services.ConfigureGenericCrud<MyContext>(o =>
    {
        // Indicate if desired use Modals 
        o.UseModals = true;
        // add the models type to manage with the package
        o.Models.Add<Models.Country>(); 
        o.Models.Add<Models.Genre>(nameof(Models.Genre.Name));
        o.Models.Add<Models.Movie, int, MovieController, Data.Context>();
    });
```

**Note:** From the version 2.0.0 the next features was marked how to obsolete and will be removed in future versions:
    - The method *o.Models.Add(Type modelType, string keyPropertyName = "Id", string controllerName = "")*.
    - The *ContextCreator* option

3. Add the next line in the **UseMvc** middleware call, this in the method **Configure** of your **Startup** class
 ```cs
 routes.MapCrudRoutes();
 ```
 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Your code should see similar to the next code
 ```cs
    app.UseMvc(routes =>
    {
        routes.MapCrudRoutes(); // add this line
        routes.MapRoute(
            name: "default",
            template: "{controller=Home}/{action=Index}/{id?}");

    });
 ```
 4. Run to app and access at the url **http://localhost:5000/[ModelName]**, sample: **http://localhost:5000/Country**. In the browser you should see a similar page to :
 ![Sample index page](Mockups/sampleIndexPage.png)

## Custom controllers
If your desired personalize your controllers, add additional actions or override the default actions, then

1. Not add the model to manage in the step 3 of the last section
2. Create a new controller the inherits from **JCTools.GenericCrud.Controllers.GenericController<TDbContext, TModel, TKey>**. sample
```cs
using System;
using JCTools.GenericCrud.Controllers;
using JCTools.GenericCrud.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Test.Controllers
{
    public class MovieController : GenericController<Data.Context, Models.Movie, int>
    {
        public MovieController(IServiceProvider serviceProvider) 
        : base(serviceProvider)
        { 
            Settings.UseModals = false;
        }
    }
}
```
3. Run to app and access at the url **http://localhost:5000/Movie**,


## License
[MIT License](LICENSE)
