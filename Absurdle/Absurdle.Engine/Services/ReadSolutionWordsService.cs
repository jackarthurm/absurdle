using Absurdle.DataImport.Services;
using Microsoft.Extensions.Logging;

namespace Absurdle.Engine.Services
{
    /// <summary>
    /// Stores a collection of possible solution words read from a data service
    /// </summary>
    public class ReadSolutionWordsService : IReadSolutionWordsService
    {
        private readonly IReadWordDataService _readWordDataService;
        private readonly ILogger<ReadSolutionWordsService> _logger;

        public ICollection<string> SolutionWords { get; protected set; }
            = Array.Empty<string>();

        public ReadSolutionWordsService(
            IReadWordDataService readWordDataService,
            ILogger<ReadSolutionWordsService> logger
        )
        {
            _readWordDataService = readWordDataService;
            _logger = logger;
        }

        public async Task Init(CancellationToken token = default)
        {
            SolutionWords = await _readWordDataService
                .ReadWordsAsync(token)
                .ToArrayAsync(token);

            _logger.LogInformation(
                "Read {solutionsWordsCount} possible solution words from file",
                SolutionWords.Count
            );
        }
    }
}
