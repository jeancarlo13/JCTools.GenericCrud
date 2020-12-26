namespace JCTools.GenericCrud.Models
{
    /// <summary>
    /// Defines the required properties for display a message into the view
    /// </summary>
    public class ViewMessage
    {
        /// <summary>
        /// The message to be displayed into the view
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// The css classes to be used for generate the mesage into the view
        /// </summary>
        /// <remarks>You should specify the css class just as i would if write over the html tags</remarks>
        public string CssClass { get; set; }
    }
}