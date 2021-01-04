using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace JCTools.GenericCrud.Models
{
    /// <summary>
    /// Defines the minimum required data for represent the entities into the views
    /// </summary>
    public interface IViewModel
    {
        /// <summary>
        /// The process where the current instance to be used
        /// </summary>
        CrudProcesses CurrentProcess { get; set; }

        /// <summary>
        /// The path at the layout page
        /// </summary>
        string LayoutPage { get; set; }

        /// <summary>
        /// True for use modal for the crud actions; False (default) for use separated pages
        /// </summary>
        /// <remarks>Required Bootstrap v3.3.7 &gt;= version &lt; v4.0.0</remarks>
        bool UseModals { get; set; }

        /// <summary>
        /// The title to display into the view
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// The subtitle to display into the view
        /// </summary>
        string Subtitle { get; set; }

        /// <summary>
        /// The collection of the column names to be displayed into the view
        /// </summary>
        IEnumerable<string> Columns { get; }

        /// <summary>
        /// The name of the property used how to key/id of the model
        /// </summary>
        string KeyPropertyName { get; }

        /// <summary>
        /// True if the current CRUD use a custom controller; 
        /// False if use the <see cref="Controllers.GenericController"/> class
        /// </summary>
        bool UseCustomController { get; }

        /// <summary>
        /// Allows get the type of the model represented into the views
        /// </summary>
        /// <returns>The found type</returns>
        Type GetModelType();

        /// <summary>
        /// Allows get the localized name of the model represented into the views
        /// </summary>
        /// <returns>The found name</returns>
        string GetModelName();

        /// <summary>
        /// Allows get the type of the model id/key property
        /// </summary>
        /// <returns>The found type</returns>
        Type GetKeyType();

        /// <summary>
        /// Allows get the value of the Id/Key property of the entity
        /// </summary>
        /// <returns>The found value</returns>
        object GetId();

        /// <summary>
        /// Allows set/change the Id/Key property value of the entity to be displayed into the view
        /// </summary>
        /// <param name="id">The Id/Key property value to be set</param>
        void SetId(object id);

        /// <summary>
        /// Allows get the Key/Id property value of the specific instance
        /// </summary>
        /// <param name="obj">The instance to be evaluated</param>
        /// <returns>The found Key/Id property value or null</returns>
        object GetKeyPropertyValue(object obj);

        /// <summary>
        /// Allows get the entire data of the entity to be displayed into the view
        /// </summary>
        /// <returns>The stored data</returns>
        IEntityData GetData();

        /// <summary>
        /// Allows set/change the entire data of the entity to be displayed into the view
        /// getting from the specified source
        /// </summary>
        /// <param name="source">The <see cref="DbContext"/> the have access to the entity data</param>
        /// <param name="id">The id of the desired entity</param>
        /// <returns>The task to be executed</returns>
        Task SetDataAsync(DbContext source, string id);
    }
}