namespace Absurdle.Engine.Services
{
    public class PermutationsGenerator<T> : IPermutationsGenerator<T>
    {
        private readonly IEnumerable<T> _sequence;

        public PermutationsGenerator(IEnumerable<T> sequence)
        {
            _sequence = sequence;
        }

        /// <summary>
        /// Recursive algorithm for computing permutations of objects of a type <typeparamref name="T"/>
        /// Inspired by https://stackoverflow.com/a/25824818
        /// (the question talks about combinations but it's mistaken, the solution is for permutations)
        /// </summary>
        /// <param name="count">The number of elements to draw from the sequence. Any positive integer is valid.</param>
        /// <returns></returns>
        public IEnumerable<IEnumerable<T>> ComputePermutationsWithRepetition(uint count)
        {
            if (count == 0)
                yield return Enumerable.Empty<T>();

            else
            {
                foreach (T element in _sequence)
                    foreach (IEnumerable<T> remainingElements in ComputePermutationsWithRepetition(count - 1))
                        yield return remainingElements.Prepend(element);
            }
        }
    }
}
