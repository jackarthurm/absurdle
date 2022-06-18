using Absurdle.DataImport.Services;
using Absurdle.Engine;
using Absurdle.Engine.Services;
using CsvHelper.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Globalization;

const string validGuessesPath = "Data/valid_guesses.csv";
const string possibleSolutionsPath = "Data/possible_solutions.csv";

using Stream validGuesses = File.Open(validGuessesPath, FileMode.Open);
using Stream possibleSolutions = File.Open(possibleSolutionsPath, FileMode.Open);

await new HostBuilder()
    .ConfigureLogging(configureLogging => configureLogging
        .SetMinimumLevel(LogLevel.Information)
        .AddConsole()
        .AddDebug()
    )
    .ConfigureServices(services => services
        .AddTransient<CsvConfiguration>(
            sp => new(CultureInfo.InvariantCulture) { HasHeaderRecord = false }
        )
        .AddTransient<IGuessWordValidatorService, GuessWordValidatorService>(
            sp => new(
                new ReadWordCsvDataService(
                    validGuesses,
                    sp.GetRequiredService<CsvConfiguration>()
                ),
                new CaseInsensitiveStringComparer(),
                sp.GetRequiredService<ILogger<GuessWordValidatorService>>()
            )
        )
        .AddTransient<IReadSolutionWordsService, ReadSolutionWordsService>(
            sp => new(
                new ReadWordCsvDataService(
                    possibleSolutions,
                    sp.GetRequiredService<CsvConfiguration>()
                ),
                sp.GetRequiredService<ILogger<ReadSolutionWordsService>>()
            )
        )
        .AddTransient<IEngine, Engine>()
        .AddTransient<IPermutationsGenerator<CharacterComparisonResult>, PermutationsGenerator<CharacterComparisonResult>>(
            sp => new(
                new CharacterComparisonResult[] {
                    CharacterComparisonResult.NoMatch,
                    CharacterComparisonResult.ValueMatches,
                    CharacterComparisonResult.PositionAndValueMatches
                }
            )
        ).AddHostedService<>
    )
    .RunConsoleAsync();

        // Create the equivalence classes hashmap
        /*            IEnumerable<IEnumerable<CharacterComparisonResult>> permutations
                    = _permutationsService.ComputePermutationsWithRepetition(_wordSize);

                    foreach (IEnumerable<CharacterComparisonResult> permutation in permutations)
                        _equivalenceClasses[permutation.ToArray()] = new List<string>();

                    _logger.LogInformation(
                        "Computed {numEquivalenceClasses} initial equivalence classes",
                        _equivalenceClasses.Count
                    );*/
    }
}