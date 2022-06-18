using Absurdle.DataImport.Services;
using Microsoft.Extensions.Logging;

namespace Absurdle.Engine.Services
{
    /// <summary>
    /// Stores an array of possible solution words read from a data service
    /// </summary>
    public class ReadSolutionWordsService : IReadSolutionWordsService
    {
        private readonly IReadWordDataService _readInitialSolutionWordsService;
        private readonly ILogger<ReadSolutionWordsService> _logger;

        private ICollection<string> _solutionWords = Array.Empty<string>();

        public IEnumerable<string> SolutionWords => _solutionWords;

        public ReadSolutionWordsService(
            IReadWordDataService initialSolutionsService,
            ILogger<ReadSolutionWordsService> logger
        )
        {
            _readInitialSolutionWordsService = initialSolutionsService;
            _logger = logger;
        }

        public async Task Init()
        {
            _solutionWords = await _readInitialSolutionWordsService
                .ReadWordsAsync()
                .ToArrayAsync();

            _logger.LogInformation(
                "Read {solutionsWordsCount} possible solution words from file",
                _solutionWords.Count
            );
        }
    }
}
