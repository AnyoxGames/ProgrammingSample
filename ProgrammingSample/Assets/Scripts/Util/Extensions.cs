using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AnyoxGames.Util
{
    public static class Extensions
    {
        public static bool IsNullOrEmpty<T>(this ICollection<T> list)
        {
            if (list == null) return true;
            if (list.Count == 0) return true;
            return false;
        }

        public static T GetRandom<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
                return default(T);

            var list = enumerable as IList<T> ?? enumerable.ToList();
            if (list.Count == 0)
                return default(T);

            return list[Random.Range(0, list.Count)];
        }
    }
}