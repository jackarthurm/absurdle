using Absurdle.Engine.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Absurdle.Solver.Services
{
    internal class AbsurdleSolverService : BackgroundService
    {
        private readonly IReadSolutionWords _readSolutionWordsService;
        private readonly IGuessWordValidator _guessWordValidatorService;
        private readonly Func<ICollection<string>, IAbsurdleEngineWithMetrics> _absurdleEngineFactory;
        private readonly IConsole _consoleService;
        private readonly ILogger<AbsurdleSolverService> _logger;

        public AbsurdleSolverService(
            IReadSolutionWords readSolutionWordsService,
            IGuessWordValidator guessWordValidatorService,
            Func<ICollection<string>, IAbsurdleEngineWithMetrics> absurdleEngineFactory,
            IConsole consoleService,
            ILogger<AbsurdleSolverService> logger
        )
        {
            _readSolutionWordsService = readSolutionWordsService;
            _guessWordValidatorService = guessWordValidatorService;
            _absurdleEngineFactory = absurdleEngineFactory;
            _consoleService = consoleService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _readSolutionWordsService.Init(stoppingToken);
            await _guessWordValidatorService.Init(stoppingToken);

            ICollection<string> solutions = _readSolutionWordsService.SolutionWords;

            // Compute the number of equivalence classes after guessing each solution word
            // on a fresh absurdle engine
            IEnumerable<Task<Tuple<string, int, int>>> tasks = solutions.Select(
                async word =>
                {
                    IAbsurdleEngineWithMetrics absurdleEngine = _absurdleEngineFactory(solutions);

                    await absurdleEngine.AddGuess(word, stoppingToken);

                    return Tuple.Create(
                        word, 
                        absurdleEngine.WordHintsCount, 
                        absurdleEngine.PossibleSolutionsCount
                    );
                }
            );

            IEnumerable<Tuple<string, int, int>> results = await Task.WhenAll(tasks);

            Tuple<string, int, int>? mostEquivalenceClasses = results.MaxBy(t => t.Item2);
            Tuple<string, int, int>? smallestEquivalenceClass = results.MinBy(t => t.Item3);

            _consoleService.WriteLine(
                "The word that produces the biggest number of equivalence classes after one guess is " +
                $"\"{mostEquivalenceClasses?.Item1}\" ({mostEquivalenceClasses?.Item2})"
            );
            _consoleService.WriteLine($"The word that produces the smallest largest equivalence class after one guess is " +
                $"\"{smallestEquivalenceClass?.Item1}\" ({smallestEquivalenceClass?.Item3})");
        }
    }
}
