namespace Absurdle.Engine
{
    /// <summary>
    /// An equality comparer for enumerables.
    /// This makes it possible to hash an enumerable for use as 
    /// a dictionary key or adding to a hashset.
    /// </summary>
    internal class EnumerableEqualityComparer<T> : IEqualityComparer<IEnumerable<T>>
    {
        public bool Equals(IEnumerable<T>? x, IEnumerable<T>? y)
        {
            if (x is null || y is null)
                return x is null && y is null;

            return x.Zip(y).All(pair => pair.First.Equals(pair.Second));
        }

        /// <summary>
        /// The hash of the enumerable is computed from the combined hashes of its elements
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
