namespace Absurdle.Engine.Services
{
    public interface IReadSolutionWordsService
    {
        public Task Init(CancellationToken token = default);
        public ICollection<string> SolutionWords { get; }
    }
}
