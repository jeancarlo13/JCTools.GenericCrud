using System;
using JCTools.GenericCrud.Models;

namespace JCTools.GenericCrud.Settings
{
    public interface IControllerOptions : IOptions
    {

        ICrudList ListOptions { get; set; }
        ICrudDetails DetailsOptions { get; set; }
        ICrudEdit EditOptions { get; set; }
        ICrudEdit CreateOptions { get; set; }
        ICrudDetails DeleteOptions { get; set; }
        string KeyPropertyName { get; set; }
        Type GetModelType();
    }
}