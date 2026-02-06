using IndigoLabsAssignment.Models;

namespace IndigoLabsAssignment.Utilities
{
    public static class EnumerableUtils
    {
        public static IEnumerable<T> ApplySort<T, TKey>(
            IEnumerable<T> source,
            SortOrder sortOrder,
            Func<T, TKey> predicate
        )
        {
            return sortOrder.Equals(SortOrder.Asc)
                ? source.OrderBy(predicate)
                : source.OrderByDescending(predicate);
        }
    }
}
