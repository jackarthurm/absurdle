using Absurdle.DataImport.Services;
using Microsoft.Extensions.Logging;

namespace Absurdle.Engine.Services
{
    /// <summary>
    /// Stores an array of valid guess words read from a data service
    /// Allows validating words against the valid guess words collection
    /// </summary>
    public class GuessWordValidatorService : IGuessWordValidator
    {
        private readonly IReadWordData _validGuessWordsService;
        private readonly IEqualityComparer<string> _wordComparer;
        private readonly ILogger<GuessWordValidatorService> _logger;

        private ICollection<string> _validGuessWords = Array.Empty<string>();

        public GuessWordValidatorService(
            IReadWordData validGuessWordsService,
            IEqualityComparer<string> wordComparer,
            ILogger<GuessWordValidatorService> logger
        )
        {
            _validGuessWordsService = validGuessWordsService;
            _wordComparer = wordComparer;
            _logger = logger;
        }

        public async Task Init(CancellationToken token = default)
        {
            _validGuessWords = await _validGuessWordsService
                .ReadWordsAsync(token)
                .ToArrayAsync(token);

            _logger.LogInformation(
                "Read {validGuessesCount} valid guess words from file",
                _validGuessWords.Count
            );
        }

        public bool IsValid(string guess)
            => _validGuessWords.Contains(guess, _wordComparer);
    }
}
