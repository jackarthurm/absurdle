using Absurdle.Engine.Model;
using Absurdle.Engine.Utils;
using Microsoft.Extensions.Logging;

namespace Absurdle.Engine.Services
{
    public class AbsurdleEngine : IAbsurdleEngine
    {
        private readonly ILogger<AbsurdleEngine> _logger;
        private readonly IReadSolutionWordsService _readSolutionWordsService;
        private readonly IGuessWordValidatorService _guessValidatorService;

        private IEnumerable<string> _currentPossibleSolutions = Enumerable.Empty<string>();

        public IEnumerable<CharacterHint> WordHint { get; protected set; }
            = Enumerable.Empty<CharacterHint>();

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
            // The mapping of each word hint (i.e. equivalence class) to
            // its associated collection of posible solution words
            IDictionary<IEnumerable<CharacterHint>, ICollection<string>> wordHintsToPossibleSolutions
                = new Dictionary<IEnumerable<CharacterHint>, ICollection<string>>(
                    new EnumerableEqualityComparer<CharacterHint>()
                );

            // For every solution, compute the solution's new equivalence class based on the new guess
            foreach (string solution in _currentPossibleSolutions)
            {
                IEnumerable<CharacterHint> wordHint = ComputeWordHint(guess, solution);

                // Ensure this equivalence class exists in the dictionary
                wordHintsToPossibleSolutions.TryAdd(wordHint, new HashSet<string>());

                // Add the solution to the equivalence class
                wordHintsToPossibleSolutions[wordHint].Add(solution);
            }

            _logger.LogInformation(
                "Computed {equivalenceClassesCount} bucket(s)",
                wordHintsToPossibleSolutions.Count
            );

            WordHint = ChooseNextWordHint(wordHintsToPossibleSolutions);

            // Update the new possible solutions to be the members of the largest equivalence class
            _currentPossibleSolutions = wordHintsToPossibleSolutions[WordHint];

            _logger.LogInformation(
                "The largest bucket contains {currentPossibleSolutionsCount} possible solutions",
                _currentPossibleSolutions.Count()
            );
        }

        /// <summary>
        /// Finds the largest equivalence class and returns it
        /// TODO: add a policy for choosing between equivalence classes of equal size
        /// A good policy might be to discard the winning equivalence class and select 
        /// at random from the remaining
        /// </summary>
        /// <param name="wordHintsToPossibleSolutions"></param>
        /// <returns></returns>
        private static IEnumerable<CharacterHint> ChooseNextWordHint(
            IDictionary<IEnumerable<CharacterHint>,
            ICollection<string>> wordHintsToPossibleSolutions
        ) => wordHintsToPossibleSolutions.MaxBy(pair => pair.Value.Count).Key;

        /// <summary>
        /// Takes a solution and classifies it against a guess
        /// The guess and solution must be of the same length
        /// TODO: The original absurdle doesn't seem to mark duplicate 
        /// characters both yellow whereas this one does
        /// </summary>
        /// <param name="guess"></param>
        /// <param name="solution"></param>
        private static IEnumerable<CharacterHint> ComputeWordHint(string guess, string solution)
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
