namespace JCTools.GenericCrud.Models
{
    /// <summary>
    /// The possible processes of a CRUD
    /// </summary>
    public enum CrudProcesses
    {
        /// <summary>
        /// No represent any process
        /// </summary>
        None,
        /// <summary>
        /// Represents the Index view or the "Go To Index" action
        /// </summary>        
        Index,
        /// <summary>
        /// Represents the new entity process
        /// </summary>        
        Create,
        /// <summary>
        /// Represents the save entity process
        /// </summary>        
        Save,
        /// <summary>
        /// Represents the process to be display the entity details
        /// </summary>        
        Details,
        /// <summary>
        /// Represents the edit process
        /// </summary>        
        Edit,
        /// <summary>
        /// Represents the delete entity process
        /// </summary>        
        Delete
    }
}