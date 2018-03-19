namespace JCTools.GenericCrud.Models
{
    /// <summary>
    /// The supported user messages for the <see cref="GenericCrud.Controllers.GenericController{TDbContext, TModel, TKey}.Index"/> action
    /// </summary>
    public enum IndexMessages
    {
        None = 0,
        EditSuccess = 1,
        CreateSuccess = 2,
        DeleteSuccess = 3
    }
}