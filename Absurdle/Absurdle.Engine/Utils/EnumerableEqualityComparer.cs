namespace Absurdle.Engine.Utils
{
    /// <summary>
    /// An equality comparer for enumerables.
    /// This makes it possible to hash an enumerable for use as 
    /// a dictionary key or as a hashset member.
    /// </summary>
    internal class EnumerableEqualityComparer<T> : IEqualityComparer<IEnumerable<T>>
    {
        /// <summary>
        /// Delegates to the default equality comparer of each item.
        /// </summary>
        /// <param name="x">LHS enumerable of <typeparamref name="T"/></param>
        /// <param name="y">RHS enumerable of <typeparamref name="T"/></param>
        /// <returns></returns>
        public bool Equals(IEnumerable<T>? x, IEnumerable<T>? y)
        {
            if (x is null || y is null)
                return x is null && y is null;

            return x.SequenceEqual(y);
        }

        /// <summary>
        /// The hash of the enumerable is computed from 
        /// the combined hashes of its elements.
        /// </summary>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public int GetHashCode(IEnumerable<T> enumerable)
        {
            HashCode hashCode = new();

            foreach (T item in enumerable)
                hashCode.Add(item);

            return hashCode.ToHashCode();
        }
    }
}
