// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SharpSanitizer.Helper
{
    /// <summary>
    /// Some useful methods and extension methods
    /// </summary>
    public static class GenericsHelper
    {
        public static bool Implements<T>(this Type type)
        {
            return typeof(T).IsAssignableFrom(type);
        }

        public static bool IsEnumerable(this Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type);
        }

        public static bool Implements<T>(this object data)
        {
            return data.GetType().GetInterfaces().Contains(typeof(T));
        }

        public static IEnumerable ToList<T>(this object data)
        {
            return (data as IEnumerable).Cast<T>().ToList();
        }

        public static object ConvertList(List<object> value, Type type)
        {
            var containedType = type.GenericTypeArguments.First();
            return value.Select(item => Convert.ChangeType(item, containedType)).ToList();
        }
    }
}
