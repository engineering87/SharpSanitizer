// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System.Collections.Generic;

namespace SharpSanitizer
{
    /// <summary>
    /// SharpSanitizer main interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISharpSanitizer<T>
    {
        /// <summary>
        /// Sanitizes the properties of the object.
        /// </summary>
        /// <param name="obj"></param>
        void Sanitize(T obj);
        /// <summary>
        /// Sanitizes the properties of objects within the list.
        /// </summary>
        /// <param name="list"></param>
        void Sanitize(IEnumerable<T> list);
    }
}
