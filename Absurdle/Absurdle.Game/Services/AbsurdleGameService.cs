using Absurdle.Engine.Model;
using Absurdle.Engine.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Absurdle.Game.Services
{
    internal class AbsurdleGameService : BackgroundService
    {
        private const string _textColorResetEscapeSequence = "\u001b[0m";
        private const string _textColorGreenEscapeSequence = "\u001b[32m";
        private const string _textColorYellowEscapeSequence = "\u001b[33m";

        // Maps each comparison result to its string representation on the console
        private readonly IDictionary<CharacterHint, string> _characterResults
            = new Dictionary<CharacterHint, string>()
            {
                {CharacterHint.PositionAndValueMatches, $"{_textColorGreenEscapeSequence}G{_textColorResetEscapeSequence}" },
                {CharacterHint.ValueMatches, $"{_textColorYellowEscapeSequence}Y{_textColorResetEscapeSequence}"},
                {CharacterHint.NoMatch, "B"},
            };

        private readonly Func<ICollection<string>, IAbsurdleEngine> _absurdleEngineFactory;
        private readonly IConsole _consoleService;
        private readonly ILogger<AbsurdleGameService> _logger;
        private readonly IReadSolutionWords _readSolutionWordsService;
        private readonly IGuessWordValidator _guessWordValidatorService;

        public AbsurdleGameService(
            Func<ICollection<string>, IAbsurdleEngine> absurdleEngineFactory,
            IReadSolutionWords readSolutionWordsService,
            IGuessWordValidator guessWordValidatorService,
            IConsole consoleService,
            ILogger<AbsurdleGameService> logger
        )
        {
            _absurdleEngineFactory = absurdleEngineFactory;
            _consoleService = consoleService;
            _logger = logger;
            _readSolutionWordsService = readSolutionWordsService;
            _guessWordValidatorService = guessWordValidatorService;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _readSolutionWordsService.Init(stoppingToken);
            await _guessWordValidatorService.Init(stoppingToken);

            IAbsurdleEngine absurdleEngine = _absurdleEngineFactory(
                _readSolutionWordsService.SolutionWords
            );

            // Accept valid guesses until the engine is solved
            ushort guessesCount = 0;

            do
            {
                // Accept guess inputs until a valid guess input is entered
                bool guessIsValid = false;
                string? guess;

                do
                {
                    _consoleService.WriteLine("Enter a guess:");
                    guess = _consoleService.ReadLine();

                    if (guess is null)
                        return; // No more lines are available to read, the game is over

                    guessIsValid = await absurdleEngine.AddGuess(guess, stoppingToken);

                    if (!guessIsValid)
                        _consoleService.WriteLine($"Guess \"{guess}\" was invalid");
                }
                while (!stoppingToken.IsCancellationRequested && !guessIsValid);

                ++guessesCount;

                if (absurdleEngine.BestWordHint is null)
                {
                    _logger.LogError("The absurdle engine failed to find a word hint");

                    throw new ApplicationException("Engine error");
                }

                // Print the current result
                _consoleService.WriteLine(guess);
                _consoleService.WriteLine(
                    string.Join(
                        string.Empty,
                        absurdleEngine.BestWordHint.Select(result => _characterResults[result])
                    )
                );
            }
            while (!absurdleEngine.IsSolved);

            _consoleService.WriteLine($"You beat absurdle in {guessesCount} guesses");
        }
    }
}
