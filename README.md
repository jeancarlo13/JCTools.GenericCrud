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

You can use the [editor on GitHub](https://github.com/jeancarlo13/JCTools.GenericCrud/edit/master/README.md) to maintain and preview the content for your website in Markdown files.

Whenever you commit to this repository, GitHub Pages will run [Jekyll](https://jekyllrb.com/) to rebuild the pages in your site, from the content in your Markdown files.

### Markdown

Markdown is a lightweight and easy-to-use syntax for styling your writing. It includes conventions for

```markdown
Syntax highlighted code block

# Header 1
## Header 2
### Header 3

- Bulleted
- List

1. Numbered
2. List

**Bold** and _Italic_ and `Code` text

[Link](url) and ![Image](src)
```

For more details see [GitHub Flavored Markdown](https://guides.github.com/features/mastering-markdown/).

### Jekyll Themes

Your Pages site will use the layout and styles from the Jekyll theme you have selected in your [repository settings](https://github.com/jeancarlo13/JCTools.GenericCrud/settings). The name of this theme is saved in the Jekyll `_config.yml` configuration file.

### Support or Contact

Having trouble with Pages? Check out our [documentation](https://help.github.com/categories/github-pages-basics/) or [contact support](https://github.com/contact) and we’ll help you sort it out.
