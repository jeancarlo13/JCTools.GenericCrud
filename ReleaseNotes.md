# JCTools.GenericCrud Release notes

## Content
- [JCTools.GenericCrud Release notes](#jctoolsgenericcrud-release-notes)
  - [Content](#content)
  - [Version 2.2.2](#version-222)
  - [Version 2.2.1](#version-221)
  - [Version 2.2.0](#version-220)
  - [Version 2.1.0](#version-210)
  - [Version 2.0.0](#version-200)

## Version 2.2.2
* Upgrade NewtonSoft to v13.0.2
* Fixed js bug with undeclared variable
* Added missing I18N label

## Version 2.2.1
* Added feature to check if awesome font 5 is loaded.
    * If the font is not loaded, all buttons with font icons are replaced by the configured text labels
## Version 2.2.0
* API rest added for the CRUD actions.  
* Support by JSON and XML responses added.

## Version 2.1.0
* Authorization policy support to manage access to CRUD controllers
* Fixed bug with the font awesome and its generated svg files

## Version 2.0.0
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