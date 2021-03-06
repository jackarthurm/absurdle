using CsvHelper;
using CsvHelper.Configuration;
using System.Runtime.CompilerServices;

namespace Absurdle.DataImport.Services
{
    /// <summary>
    /// Reads Absurdle word data from CSV files
    /// Assumes one word per line
    /// TODO: error handling for bad data
    /// </summary>
    public class ReadWordCsvDataService : IReadWordData, IDisposable
    {
        private readonly TextReader _reader;
        private readonly CsvReader _csvReader;

        public ReadWordCsvDataService(
            Stream source,
            CsvConfiguration csvConfiguration
        )
        {
            _reader = new StreamReader(source);
            _csvReader = new(_reader, csvConfiguration);
        }

        public async IAsyncEnumerable<string> ReadWordsAsync(
            [EnumeratorCancellation] CancellationToken token = default
        )
        {
            while (await _csvReader.ReadAsync())
                yield return _csvReader.GetField<string>(0);
        }

        public IEnumerable<string> ReadWords()
        {
            while (_csvReader.Read())
                yield return _csvReader.GetField<string>(0);
        }

        public void Dispose()
        {
            _csvReader.Dispose();
            _reader.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
