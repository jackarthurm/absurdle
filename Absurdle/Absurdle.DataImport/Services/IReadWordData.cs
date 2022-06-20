namespace Absurdle.DataImport.Services
{
    public interface IReadWordData
    {
        public IAsyncEnumerable<string> ReadWordsAsync(CancellationToken token = default);
        public IEnumerable<string> ReadWords();
    }
}
