namespace JCTools.GenericCrud.Models
{
    /// <summary>
    /// The supported user messages for the <see cref="GenericCrud.Controllers.GenericController.Index"/> action
    /// </summary>
    public enum IndexMessages
    {
        /// <summary>
        /// Without message
        /// </summary>
        None = 0,
        /// <summary>
        /// The message of a successful edition
        /// </summary>        
        EditSuccess = 1,
        /// <summary>
        /// The message of a successful creation
        /// </summary>        
        CreateSuccess = 2,
        /// <summary>
        /// The message of a successful deletion
        /// </summary>                
        DeleteSuccess = 3
    }
}