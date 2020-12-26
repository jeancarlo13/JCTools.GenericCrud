using System;
using System.Collections.Generic;
using Microsoft.Extensions.Localization;

namespace JCTools.GenericCrud.Models
{
    /// <summary>
    /// Defines the required data for represent the entities into the edit view 
    /// </summary>
    public interface IEditModel : IViewModel
    {

        /// <summary>
        /// Allows set/change the entire data of the entity to be displayed into the view
        /// </summary>
        /// <param name="model">The entire data to be set</param>
        void SetData(object model);

        /// <summary>
        /// The configuration to be used for represents the icon/button of the "Go To Index" action
        /// </summary>
        CrudAction IndexAction { get; set; }

        /// <summary>
        /// The configuration to be used for represents the icon/button of the save action
        /// </summary>
        CrudAction SaveAction { get; set; }
    }
}