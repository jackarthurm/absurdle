using Absurdle.Engine.Model;
using Absurdle.Engine.Services;
using Absurdle.UI.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Absurdle.UI
{
    internal class ConsoleApplication : IHostedService
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

        private readonly IAbsurdleEngine _absurdle;
        private readonly IConsoleService _consoleService;
        private readonly ILogger<ConsoleApplication> _logger;

        public ConsoleApplication(
            IAbsurdleEngine absurdle,
            IConsoleService consoleService,
            ILogger<ConsoleApplication> logger
        )
        {
            _absurdle = absurdle;
            _consoleService = consoleService;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken token)
        {
            _logger.LogInformation("Starting absurdle engine");
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
                    if (token.IsCancellationRequested)
                        return;

                    _consoleService.WriteLine("Enter a guess:");
                    guess = _consoleService.ReadLine();

                    if (guess != null)
                        guessIsValid = await _absurdle.MakeGuess(guess, token);

                    if (!guessIsValid)
                        _consoleService.WriteLine($"Guess \"{guess}\" was invalid");
                }
                while (guess is null || !guessIsValid);

                ++guessesCount;

                // Print the current result
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

        public Task StopAsync(CancellationToken token) => Task.CompletedTask;
    }
}
