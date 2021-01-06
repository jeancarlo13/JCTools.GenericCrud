using System.Collections.Generic;
using Microsoft.Extensions.Localization;

namespace JCTools.GenericCrud.Services
{
    /// <summary>
    /// Required for the injection of the <see cref="CrudLocalizer"/> service
    /// </summary>
    public interface ICrudLocalizer : IStringLocalizer
    {
    }
}