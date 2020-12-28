using System;
using System.Collections.Generic;
#if NETCOREAPP2_1
using Microsoft.DotNet.PlatformAbstractions;
#endif

namespace JCTools.GenericCrud.Helpers
{
    /// <summary>
    /// Allows makes comparison between string arrays
    /// </summary>
    internal class StringArrayComparer : IEqualityComparer<string[]>
    {
        /// <summary>
        /// Gets a <see cref="StringArrayComparer" /> object that performs a case-sensitive ordinal string comparison.
        /// </summary>
        public static readonly StringArrayComparer Ordinal = new StringArrayComparer(StringComparer.Ordinal);

        /// <summary>
        /// Gets a <see cref="StringArrayComparer" /> object that performs a case-insensitive ordinal string comparison.
        /// </summary>
        public static readonly StringArrayComparer OrdinalIgnoreCase = new StringArrayComparer(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Represents the string comparison operation that uses specific case and culture-based or ordinal comparison rules.
        /// </summary>
        private readonly StringComparer _valueComparer;

        /// <summary>
        /// Initialize the new instance that performs ordinal string comparison. 
        /// </summary>
        /// <param name="valueComparer">The string comparison operation that uses specific case and culture-based or ordinal comparison rules.</param>
        private StringArrayComparer(StringComparer valueComparer)
            => _valueComparer = valueComparer;

        /// <summary>
        /// Makes the comparison between the received string array
        /// </summary>
        /// <param name="first">The first string array to be compared</param>
        /// <param name="second">The second string array to be compared</param>
        /// <returns>True if both string array are equals; False another case</returns>
        public bool Equals(string[] first, string[] second)
        {
            if (object.ReferenceEquals(first, second))
                return true;

            if (first == null ^ second == null)
                return false;

            if (first.Length != second.Length)
                return false;

            for (var i = 0; i < first.Length; i++)
            {
                if (string.IsNullOrEmpty(first[i]) && string.IsNullOrEmpty(second[i]))
                    continue;

                if (!_valueComparer.Equals(first[i], second[i]))
                    return false;
            }

            return true;
        }
        /// <summary>
        /// Allows get the hash code of a string array
        /// </summary>
        /// <param name="obj">The string array to be used in the hash code generation</param>
        /// <returns>The generated hash code </returns>
        public int GetHashCode(string[] obj)
        {
            if (obj == null)
                return 0;

            var hash = new System.HashCode();
            for (var i = 0; i < obj.Length; i++)
                hash.Add(obj[i], _valueComparer);

            return hash.ToHashCode();
        }
    }
}