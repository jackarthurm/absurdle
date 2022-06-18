namespace Absurdle.DataImport.Services
{
    public interface IReadWordDataService
    {
        public IAsyncEnumerable<string> ReadWordsAsync();
        public IEnumerable<string> ReadWords();
    }
}
