using System;
using System.Collections.Generic;

namespace Shared.Util
{
    public static class Extension
    {
        public static bool IsEmpty<T>(this ICollection<T> collection)
        {
            return collection.Count == 0;
        }
        public static bool IsEmpty<T>(this IReadOnlyCollection<T> collection)
        {
            return collection.Count == 0;
        }
    }
}
