namespace Absurdle.Engine.Services
{
    public interface IReadSolutionWords
    {
        public Task Init(CancellationToken token = default);
        public ICollection<string> SolutionWords { get; }
    }
}
