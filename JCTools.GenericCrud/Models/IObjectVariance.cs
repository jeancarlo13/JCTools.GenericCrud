using System.Collections.Generic;

namespace JCTools.GenericCrud.Models
{
    /// <summary>
    /// Represent a variance between two properties
    /// </summary>
    internal interface IObjectVariance
    {
        /// <summary>
        /// The name of the property with differences
        /// </summary>
        string Property { get; set; }

        /// <summary>
        /// Allows set the value of the first object
        /// </summary>
        /// <param name="value">The value to set</param>
        void SetFirstValue(object value);

        /// <summary>
        /// Allows set the value of the second object
        /// </summary>
        /// <param name="value">The value to set</param>
        void SetSecondValue(object value);

        /// <summary>
        /// Checks the setted objects for know if are equals
        /// </summary>
        /// <returns>True if are equals</returns>
        bool AreEquals();

        /// <summary>
        /// Gets the variances between the properties/fields of 
        /// the setted objects
        /// </summary>
        /// <param name="depth">The depth level to compare the children 
        /// fields/properties of the objects. If the value is 0 or less 
        /// not are reviewed the children fields/properties</param>        
        /// <returns>The found collection of variances</returns>        
        IEnumerable<IObjectVariance> GetVariances(int depth);
    }
}
