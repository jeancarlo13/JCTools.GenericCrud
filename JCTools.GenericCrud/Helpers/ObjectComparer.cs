using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JCTools.GenericCrud.Models;

namespace JCTools.GenericCrud.Helpers
{
    /// <summary>
    /// Allows compare objects and get all difference among them
    /// </summary>
    internal static class ObjectComparer
    {
        /// <summary>
        /// Gets the variances between two objects
        /// </summary>
        /// <param name="first">The first object to compare</param>
        /// <param name="second">The seconds object to compare</param>
        /// <param name="depth">The depth level to compare the children 
        /// fields/properties of the objects</param>
        /// <param name="prefix">The prefix to use for the detected variances</param>
        /// <typeparam name="T">The type of the objects to be compared</typeparam>
        /// <returns>The found collection of variances</returns>
        public static IEnumerable<IObjectVariance> DetailedCompare<T>(
            this T first,
            T second,
            int depth = 2,
            string prefix = ""
        )
        {
            var type = first.GetType();
            var varianceType = typeof(ObjectVariance<>);
            List<IObjectVariance> variances = new List<IObjectVariance>();
            FieldInfo[] fi = type.GetFields();
            foreach (FieldInfo f in fi)
            {
                try
                {
                    IObjectVariance v = Activator.CreateInstance(varianceType.MakeGenericType(f.FieldType)) as IObjectVariance;

                    v.Property = $"{prefix}.{f.Name}";
                    v.SetFirstValue(f.GetValue(first));
                    v.SetSecondValue(f.GetValue(second));
                    if (!v.AreEquals())
                    {
                        if (depth > 0)
                        {
                            var internalvariances = v.GetVariances(depth - 1);
                            if (internalvariances.Any())
                            {
                                variances.Add(v);
                                variances.AddRange(internalvariances);
                            }
                        }
                        else
                            variances.Add(v);
                    }
                }
                catch (Exception) { }
            }

            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo p in properties)
            {
                try
                {
                    IObjectVariance v = Activator.CreateInstance(varianceType.MakeGenericType(p.PropertyType)) as IObjectVariance;
                    v.Property = $"{prefix}.{p.Name}";
                    v.SetFirstValue(p.GetValue(first));
                    v.SetSecondValue(p.GetValue(second));
                    if (!v.AreEquals())
                    {
                        if (depth > 0)
                        {
                            var internalvariances = v.GetVariances(depth - 1);
                            if (internalvariances.Any())
                            {
                                variances.Add(v);
                                variances.AddRange(internalvariances);
                            }
                        }
                        else
                            variances.Add(v);
                    }
                }
                catch (Exception) { }
            }
            return variances;
        }


    }
}