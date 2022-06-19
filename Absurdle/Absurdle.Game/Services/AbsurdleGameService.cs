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

        private readonly IAbsurdleEngineService _absurdle;
        private readonly IConsoleService _consoleService;
        private readonly ILogger<AbsurdleGameService> _logger;

        public AbsurdleGameService(
            IAbsurdleEngineService absurdle,
            IConsoleService consoleService,
            ILogger<AbsurdleGameService> logger
        )
        {
            _absurdle = absurdle;
            _consoleService = consoleService;
            _logger = logger;
        }

        protected async override Task ExecuteAsync(CancellationToken token)
        {
            _logger.LogInformation("Starting absurdle engine...");
            await _absurdle.Init(token);
            _logger.LogInformation("Absurdle engine is ready");

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

                    guessIsValid = await _absurdle.MakeGuess(guess, token);

                    if (!guessIsValid)
                        _consoleService.WriteLine($"Guess \"{guess}\" was invalid");
                }
                while (!token.IsCancellationRequested && !guessIsValid);

                ++guessesCount;

                // Print the current result
                _consoleService.WriteLine(guess);
                _consoleService.WriteLine(
                    string.Join(
                        string.Empty,
                        _absurdle.WordHint.Select(result => _characterResults[result])
                    )
                );
            }
            while (!_absurdle.IsSolved);

            _consoleService.WriteLine($"You beat absurdle in {guessesCount} guesses");
        }
    }
}
