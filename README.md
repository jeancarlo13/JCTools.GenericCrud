## JCTools.GenericCrud

Simplification of the **C**reate, **R**ead, **U**pdate and **D**elete pages of the application models.

### Overview

All application required multiple pages for edited the base models. This pages generally are equals to each other.

This package allows reduce this task at minimum of actions.

You only require create and configure your models, and this package create the necessary controllers, views and actions for the **C**reate, **R**ead, **U**pdate and **D**elete actions.

### Usage

1. Add the package to your application
```bash
Install-Package JCTools.GenericCrud -Version 1.0.0

Or

dotnet add package JCTools.GenericCrud --version 1.0.0
```
2. Add the next lines in the method **ConfigureServices** of your **Startup** class
```cs
    services.ConfigureGenericCrud(o =>
    {
        o.UseModals = true;
        o.ContextCreator = () => new Test.Data.Context(); // method that will create an database context instance 
        o.Models.Add(typeof(Models.Country)); // add the model type to manage with the package
    });
```
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
 



 ### License
[MIT License](/LICENSE)
