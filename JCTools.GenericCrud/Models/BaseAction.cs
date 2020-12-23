namespace JCTools.GenericCrud.Models
{
    /// <summary>
    /// Allows defined the css classes of the icon/button of the CRUD actions 
    /// </summary>
    public class BaseAction
    {
        /// <summary>
        /// The css classes to be use into the Icon that represent a CRUD action
        /// </summary>
        /// <remarks>You should specify the css class just as i would if write over the html tags</remarks>
        public string IconClass { get; set; } = string.Empty;
        /// <summary>
        /// The css classes to be use into the Button that represent a CRUD action
        /// </summary>
        /// <remarks>You should specify the css class just as i would if write over the html tags</remarks>
        public string ButtonClass { get; set; } = string.Empty;

    }
}