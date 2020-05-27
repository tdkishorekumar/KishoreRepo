using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchFight.Utilities
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<int> Indexes<T>(this IReadOnlyCollection<T> collection)
        {
            return Enumerable.Range(0, collection.Count);
        }

        public static int MaxIndex<T>(this IEnumerable<T> source)
        {
            return source.MaxIndex(Comparer<T>.Default);
        }

        public static int MaxIndex<T>(this IEnumerable<T> source, IComparer<T> comparer)
        {
            using (var iterator = source.GetEnumerator())
            {
                if (!iterator.MoveNext())
                    throw new InvalidOperationException("Empty sequence.");

                int maxIndex = 0;
                T maxElement = iterator.Current;
                int index = 0;

                while (iterator.MoveNext())
                {
                    index++;
                    T element = iterator.Current;
                    if (comparer.Compare(element, maxElement) > 0)
                    {
                        maxElement = element;
                        maxIndex = index;
                    }
                }

                return maxIndex;
            }
        }
    }
}
