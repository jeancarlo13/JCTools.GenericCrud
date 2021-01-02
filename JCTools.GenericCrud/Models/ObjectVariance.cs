using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JCTools.GenericCrud.Helpers;

namespace JCTools.GenericCrud.Models
{

    /// <summary>
    /// Represent a variance between two properties
    /// </summary>
    internal class ObjectVariance<T> : IObjectVariance
    {
        /// <summary>
        /// The name of the property with differences
        /// </summary>
        public string Property { get; set; }

        /// <summary>
        /// The compared first value 
        /// </summary>
        public T FirstValue { get; private set; }

        /// <summary>
        /// The compared second value 
        /// </summary>
        public T SecondValue { get; private set; }

        /// <summary>
        /// Allows set the value of the first object
        /// </summary>
        /// <param name="value">The value to set</param>
        public void SetFirstValue(object value) => FirstValue = (T)value;

        /// <summary>
        /// Allows set the value of the second object
        /// </summary>
        /// <param name="value">The value to set</param>
        public void SetSecondValue(object value) => SecondValue = (T)value;

        /// <summary>
        /// Checks the setted objects for know if are equals
        /// </summary>
        /// <returns>True if are equals</returns>
        public bool AreEquals()
        {
            if (FirstValue == null ^ SecondValue == null)
                return false;
            else if (FirstValue != null)
            {
                MethodInfo specificEquals;
                if (FirstValue is IEnumerable enumerable)
                {
                    var elementType = enumerable.AsQueryable().ElementType;
                    var setType = typeof(HashSet<>).MakeGenericType(elementType);
                    var set1 = Activator.CreateInstance(setType, new object[] { FirstValue });
                    var set2 = Activator.CreateInstance(setType, new object[] { SecondValue });

                    specificEquals = setType.GetMethod("SetEquals", new Type[] { setType });
                    if (specificEquals != null && specificEquals.ReturnType == typeof(bool))
                        return (bool)specificEquals.Invoke(set1, new object[] { set2 });
                }
                else
                {  // uses Reflection to check if a Type-specific `Equals` exists...
                    specificEquals = typeof(T).GetMethod("Equals", new Type[] { typeof(T) });

                    if (specificEquals != null && specificEquals.ReturnType == typeof(bool))
                        return (bool)specificEquals.Invoke(FirstValue, new object[] { SecondValue });
                }

                return FirstValue.Equals(SecondValue);

            }

            return false;
        }

        /// <summary>
        /// Gets the variances between the properties/fields of 
        /// the setted objects
        /// </summary>
        /// <param name="depth">The depth level to compare the children 
        /// fields/properties of the objects. If the value is 0 or less 
        /// not are reviewed the children fields/properties</param>        
        /// <returns>The found collection of variances</returns>    
        public IEnumerable<IObjectVariance> GetVariances(int depth)
            => FirstValue?.DetailedCompare(SecondValue, depth, Property)
                ?? Enumerable.Empty<IObjectVariance>();
    }

    internal static class EqualsExtensors
    {

        public static bool Equals(this IReadOnlyDictionary<string, object> first, IReadOnlyDictionary<string, object> second)
            => first.Any(entry => second[entry.Key] != entry.Value);
    }
}