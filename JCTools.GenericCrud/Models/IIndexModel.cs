using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace JCTools.GenericCrud.Models
{
    /// <summary>
    /// Defines the required data for represent the entities into the Index view 
    /// </summary>
    public interface IIndexModel : IViewModel
    {
        /// <summary>
        /// True if the columns with the CRUD actions (buttons/icons)
        /// are displayed into the list of the Index view;
        /// another, false.
        /// </summary>        
        bool ShowActionsColumns { get; }

        /// <summary>
        /// The message to be displayed into the view.
        /// Used for display the success or error messages into the CRUD operations
        /// </summary>
        ViewMessage Message { get; set; }

        /// <summary>
        /// The configuration to be used for represents the icon/button 
        /// of the new entity action
        /// </summary>
        CrudAction NewAction { get; set; }

        /// <summary>
        /// The configuration to be used for represents the icon/button 
        /// of the action to be display the entity details
        /// </summary>
        CrudAction DetailsAction { get; set; }
        /// <summary>
        /// The configuration to be used for represents the icon/button of the edit action
        /// </summary>
        CrudAction EditAction { get; set; }

        /// <summary>
        /// The configuration to be used for represents the icon/button
        /// of the delete entity action
        /// </summary>
        CrudAction DeleteAction { get; set; }

        /// <summary>
        /// The url of the generic js script to be use
        /// </summary>
        string JsScriptUrl { get; }


        /// <summary>
        /// Allows get the entire data of all entities to be displayed into the view
        /// </summary>
        /// <returns>The stored data</returns>
        IEnumerable<IEntityData> GetCollectionData();

        /// <summary>
        /// Sets a collection of entities to be displayed into the view
        /// </summary>
        /// <param name="data">The entities collection to be set</param>
        void SetData(IEnumerable<object> data);
        /// <summary>
        /// Sets all entities to be displayed into the view
        /// </summary>
        /// <param name="source">the <see cref="DbContext"/> instance 
        /// to be used for get all entities</param>
        void SetData(DbContext source);
    }
}