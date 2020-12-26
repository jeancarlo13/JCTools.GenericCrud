using System.ComponentModel.DataAnnotations;
using System.Reflection;
using JCTools.GenericCrud.DataAnnotations;
using Microsoft.Extensions.Localization;

namespace JCTools.GenericCrud.Helpers
{
    /// <summary>
    /// Represents a property to be used by the CRUD views 
    /// </summary>
    public class Property
    {
        /// /// </summary>
        /// <summary>
        /// The attribute data of the represented property
        /// <value></value>
        private readonly PropertyInfo _info;

        /// <summary>
        /// The display name to be displayed to the user
        /// Returns a display name that is used for field display in the UI
        /// </summary>
        /// <remarks>
        /// The localized string for the <see cref="DisplayAttribute.Name"/> property, 
        /// if the <see cref="DisplayAttribute.ResourceType" /> property has been 
        /// specified and the <see cref="DisplayAttribute.Name" /> property 
        /// represents a resource key; otherwise, the non-localized value of 
        /// the <see cref="DisplayAttribute.Name"/> property.
        /// </remarks>
        private string _displayName;

        /// <summary>
        /// The name of the represented property
        /// </summary>
        public string Name { get => _info.Name; }

        /// <summary>
        /// Gets the display order weight of the column
        /// </summary>
        public int Order { get; }

        /// <summary>
        /// True if the property is visible in the crud list; False another case
        /// </summary>
        public bool IsVisible { get; }

        /// <summary>
        /// True if desired use a custom view; False another case.
        /// The custom views are find in the controller view folder with the name 
        /// of the property preceded by "__Details" or "__Edit"
        /// </summary>
        public bool UseCustomView { get; }

        /// <summary>
        /// Initialize the instance with the specific property info
        /// </summary>
        /// <param name="property"><see cref="PropertyInfo" /> instance used for get
        /// the property attributes and grant access to their metadata</param>
        /// <param name="localizer">The instance of <see cref="IStringLocalizer"/> used for translate 
        /// the texts to displayed into the view</param>
        public Property(PropertyInfo property, IStringLocalizer localizer)
        {
            _info = property;

            var crudAttribute = _info.GetCustomAttribute<CrudAttribute>();
            IsVisible = crudAttribute?.Visible ?? true;
            UseCustomView = crudAttribute?.UseCustomView ?? false;

            var displayAttribute = _info.GetCustomAttribute<System.ComponentModel.DataAnnotations.DisplayAttribute>();
            Order = displayAttribute?.Order ?? 0;

            _displayName = localizer.GetString(displayAttribute?.GetName() ?? Name);
        }

        /// <summary>
        /// Returns the property value of a specified object.
        /// </summary>
        /// <param name="obj">The object whose property value will be returned.</param>
        /// <returns>The property value of the specified object.</returns>
        public object GetValue(object obj)
            => _info.GetValue(obj);

        /// <summary>
        /// Return the display name of the property
        /// </summary>
        public override string ToString()
            => string.IsNullOrWhiteSpace(_displayName) ? Name : _displayName;
    }
}