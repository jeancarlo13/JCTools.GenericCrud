namespace JCTools.GenericCrud.Models
{
    /// <summary>
    /// Defines the required data for represent the entities into the details view 
    /// </summary>
    public interface IDetailsModel : IViewModel
    {
        /// <summary>
        /// Allows set/change the entire data of the entity to be displayed into the view
        /// </summary>
        /// <param name="model">The entire data to be set</param>
        void SetData(object model);

        /// <summary>
        /// The configuration to be used for represents the icon/button of the edit action
        /// </summary>
        CrudAction EditAction { get; set; }

        /// <summary>
        /// The configuration to be used for represents the icon/button of the delete action
        /// </summary>
        CrudAction DeleteAction { get; set; }

        /// <summary>
        /// The configuration to be used for represents the icon/button of the "Go To Index" action
        /// </summary>
        CrudAction IndexAction { get; set; }

    }
}