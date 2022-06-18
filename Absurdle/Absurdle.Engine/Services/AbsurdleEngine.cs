using Microsoft.Extensions.Logging;

namespace Absurdle.Engine.Services
{
    public class AbsurdleEngine : IAbsurdleEngine
    {
        private readonly ILogger<AbsurdleEngine> _logger;
        private readonly IReadSolutionWordsService _readSolutionWordsService;
        private readonly IGuessWordValidatorService _guessValidatorService;

        private IEnumerable<string> _currentPossibleSolutions = Enumerable.Empty<string>();

        public IEnumerable<CharacterHint> WordHint { get; private set; } = Array.Empty<CharacterHint>();

        public AbsurdleEngine(
            IReadSolutionWordsService readSolutionWordsService,
            IGuessWordValidatorService guessValidatorService,
            ILogger<AbsurdleEngine> logger
        )
        {
            _logger = logger;
            _readSolutionWordsService = readSolutionWordsService;
            _guessValidatorService = guessValidatorService;
        }

        public async Task Init(CancellationToken token = default)
        {
            await _guessValidatorService.Init(token);
            await _readSolutionWordsService.Init(token);

            _currentPossibleSolutions = _readSolutionWordsService.SolutionWords;
        }

        public async Task<bool> MakeGuess(string guess, CancellationToken token = default)
        {
            if (!_guessValidatorService.IsValid(guess))
                return false;

            await Task.Run(() => Update(guess), token);

            return true;
        }

        /// <summary>
        /// Updates the current possible solutions based on a new guess word
        /// Returns the new hint
        /// </summary>
        /// <param name="guess"></param>
        private void Update(string guess)
        {
            IDictionary<IEnumerable<CharacterHint>, ICollection<string>> equivalenceClasses
                = new Dictionary<IEnumerable<CharacterHint>, ICollection<string>>(
                    new EnumerableEqualityComparer<CharacterHint>()
                );

            // For every solution, compute the solution's new equivalence class based on the new guess
            foreach (string solution in _currentPossibleSolutions)
            {
                IEnumerable<CharacterHint> equivalenceClass = Classify(guess, solution);

                // Ensure this equivalence class exists in the dictionary
                equivalenceClasses.TryAdd(equivalenceClass, new HashSet<string>());

                // Add the solution to the equivalence class
                equivalenceClasses[equivalenceClass].Add(solution);
            }

            _logger.LogInformation(
                "Computed {equivalenceClassesCount} bucket(s)",
                equivalenceClasses.Count
            );

            WordHint = ChooseNextWordHint(equivalenceClasses);

            // Update the new possible solutions to be the members of the largest equivalence class
            _currentPossibleSolutions = equivalenceClasses[WordHint];

            _logger.LogInformation(
                "The largest bucket contains {currentPossibleSolutionsCount} possible solutions",
                _currentPossibleSolutions.Count()
            );
        }

        /// <summary>
        /// Chooses a new 
        /// </summary>
        /// <param name="equivalenceClasses"></param>
        /// <returns></returns>
        private IEnumerable<CharacterHint> ChooseNextWordHint(
            IDictionary<IEnumerable<CharacterHint>, ICollection<string>> equivalenceClasses
        )
        {
            // Find the largest equivalence class
            // TODO: add a policy for choosing between equivalence classes of equal size
            return equivalenceClasses.MaxBy(pair => pair.Value.Count).Key;
        }

        /// <summary>
        /// Takes a solution and classifies it against a guess
        /// The guess and solution must be of the same length
        /// </summary>
        /// <param name="guess"></param>
        /// <param name="solution"></param>
        private static IEnumerable<CharacterHint> Classify(string guess, string solution)
        {
            for (int i = 0; i < guess.Length; ++i)
            {
                if (char.ToUpperInvariant(guess[i]).Equals(char.ToUpperInvariant(solution[i])))
                    yield return CharacterHint.PositionAndValueMatches;

                else if (solution.Contains(guess[i], StringComparison.OrdinalIgnoreCase))
                    yield return CharacterHint.ValueMatches;

                else
                    yield return CharacterHint.NoMatch;
            }
        }
    }
}
