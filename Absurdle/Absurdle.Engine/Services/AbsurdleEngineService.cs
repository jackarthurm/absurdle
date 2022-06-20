using Absurdle.Engine.Model;
using Absurdle.Engine.Utils;
using Microsoft.Extensions.Logging;

namespace Absurdle.Engine.Services
{
    public class AbsurdleEngineService : IAbsurdleEngineWithMetrics
    {
        private readonly ILogger<AbsurdleEngineService> _logger;
        private readonly IGuessWordValidator _guessWordValidatorService;

        protected ICollection<string> PossibleSolutions { get; set; }

        public int PossibleSolutionsCount => PossibleSolutions.Count;
        public int WordHintsCount { get; protected set; } = 0;

        public IEnumerable<CharacterHint>? BestWordHint { get; protected set; }

        public AbsurdleEngineService(
            ICollection<string> possibleSolutions,
            IGuessWordValidator guessWordValidatorService,
            ILogger<AbsurdleEngineService> logger
        )
        {
            _guessWordValidatorService = guessWordValidatorService;
            _logger = logger;

            PossibleSolutions = possibleSolutions;
        }

        public async Task<bool> AddGuess(string guess, CancellationToken token = default)
        {
            if (!_guessWordValidatorService.IsValid(guess))
                return false;

            await Task.Run(
                () => PossibleSolutions = PruneSolutions(PossibleSolutions, guess),
                token
            );

            return true;
        }

        /// <summary>
        /// Prunes a collection of possible solutions to be the members of the "best" equivalence class
        /// </summary>
        /// <param name="guess"></param>
        private ICollection<string> PruneSolutions(ICollection<string> solutions, string guess)
        {
            // Classify the current possible solutions against the guess
            IDictionary<IEnumerable<CharacterHint>, ICollection<string>> wordHintBuckets
                = ComputeWordHintBuckets(solutions, guess);

            WordHintsCount = wordHintBuckets.Count;

            _logger.LogInformation(
                "Computed {WordHintsCount} bucket(s)",
                WordHintsCount
            );

            BestWordHint = ChooseBestWordHint(wordHintBuckets);

            _logger.LogInformation(
                "The largest bucket contains {PossibleSolutionsCount} possible solutions",
                PossibleSolutionsCount
            );

            return wordHintBuckets[BestWordHint];
        }

        protected virtual IDictionary<IEnumerable<CharacterHint>, ICollection<string>> ComputeWordHintBuckets(
            IEnumerable<string> solutions,
            string guess
        )
        {
            // The mapping of each word hint (i.e. equivalence class) to
            // its associated collection of possible solution words
            IDictionary<IEnumerable<CharacterHint>, ICollection<string>> wordHintBuckets
                = new Dictionary<IEnumerable<CharacterHint>, ICollection<string>>(
                    new EnumerableEqualityComparer<CharacterHint>()
                );

            // For every solution, compute the solution's new equivalence class based on the new guess
            foreach (string solution in solutions)
            {
                IEnumerable<CharacterHint> wordHint = ComputeWordHint(guess, solution);

                // Ensure this equivalence class exists in the dictionary
                wordHintBuckets.TryAdd(wordHint, new HashSet<string>());

                // Add the solution to the equivalence class
                wordHintBuckets[wordHint].Add(solution);
            }

            return wordHintBuckets;
        }

        /// <summary>
        /// Finds the largest equivalence class and returns it
        /// TODO: add a policy for choosing between equivalence classes of equal size
        /// A good policy might be to discard the winning equivalence class and select 
        /// at random from the remaining
        /// </summary>
        /// <param name="wordHints"></param>
        /// <returns></returns>
        protected virtual IEnumerable<CharacterHint> ChooseBestWordHint(
            IDictionary<
                IEnumerable<CharacterHint>,
                ICollection<string>
            > wordHintBuckets
        ) => wordHintBuckets.MaxBy(pair => pair.Value.Count).Key;

        /// <summary>
        /// Takes a solution and compares it against a guess to obtain a word hint
        /// The guess and solution must be of the same length
        /// TODO: The original absurdle doesn't seem to mark duplicate 
        /// characters both yellow whereas this one does
        /// </summary>
        /// <param name="guess"></param>
        /// <param name="solution"></param>
        protected virtual IEnumerable<CharacterHint> ComputeWordHint(string guess, string solution)
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
