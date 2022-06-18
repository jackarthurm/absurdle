namespace Absurdle.Engine.Services
{
    public interface IPermutationsGenerator<T>
    {
        public IEnumerable<IEnumerable<T>> ComputePermutationsWithRepetition(uint count);
    }
}
