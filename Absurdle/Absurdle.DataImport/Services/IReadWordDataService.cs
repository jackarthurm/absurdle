namespace Absurdle.DataImport.Services
{
    public interface IReadWordDataService
    {
        public IAsyncEnumerable<string> ReadWordsAsync(CancellationToken token = default);
        public IEnumerable<string> ReadWords();
    }
}
