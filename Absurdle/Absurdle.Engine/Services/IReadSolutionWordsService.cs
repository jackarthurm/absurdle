namespace Absurdle.Engine.Services
{
    public interface IReadSolutionWordsService
    {
        public Task Init();
        public IEnumerable<string> SolutionWords { get; }
    }
}
