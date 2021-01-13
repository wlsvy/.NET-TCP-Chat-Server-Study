using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Util
{
    public static class Extension
    {
        public static bool Empty<T>(this ICollection<T> collection)
        {
            return collection.Count == 0;
        }
        public static bool IsEmpty<T>(this IReadOnlyCollection<T> collection)
        {
            return collection.Count == 0;
        }
    }
}
