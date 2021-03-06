using Absurdle.DataImport.Services;
using Absurdle.Engine.Services;
using Absurdle.Game.Services;
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
        .AddSingleton<IGuessWordValidator, GuessWordValidatorService>(
            sp => new(
                new ReadWordCsvDataService(
                    validGuesses,
                    sp.GetRequiredService<CsvConfiguration>()
                ),
                StringComparer.InvariantCultureIgnoreCase,
                sp.GetRequiredService<ILogger<GuessWordValidatorService>>()
            )
        )
        .AddSingleton<IReadSolutionWords, ReadSolutionWordsService>(
            sp => new(
                new ReadWordCsvDataService(
                    possibleSolutions,
                    sp.GetRequiredService<CsvConfiguration>()
                ),
                sp.GetRequiredService<ILogger<ReadSolutionWordsService>>()
            )
        )
        .AddTransient<Func<ICollection<string>, IAbsurdleEngine>>(
            sp => solutions => new AbsurdleEngineService(
                solutions,
                sp.GetRequiredService<IGuessWordValidator>(),
                sp.GetRequiredService<ILogger<AbsurdleEngineService>>()
            )
        )
        .AddTransient<IConsole, ConsoleService>()
        .AddHostedService<AbsurdleGameService>()
    )
    .RunConsoleAsync();
