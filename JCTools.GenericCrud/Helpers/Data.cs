namespace JCTools.GenericCrud.Helpers
{
    /// <summary>
    /// Represents the data of a property of the entity to be used into the CRUD
    /// </summary>
    public class Data
    {
        /// <summary>
        /// The name of the represented property 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The display string to be used instead of the property name
        /// </summary>
        /// <remarks>Normally, it's a localized string</remarks>
        public string Display { get; set; }
        /// <summary>
        /// The real value of the property.
        /// Represents the data that is actually manipulated
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// True if the current represented property is visible for the user;
        /// False another case
        /// </summary>
        public bool Visible { get; set; }
        /// <summary>
        /// True if for the represented property will used a custom view
        /// </summary>
        public bool UseCustomView { get; set; }

        /// <summary>
        /// The name of the represented property 
        /// </summary>
        public override string ToString() => Name;
    }
}