﻿using Absurdle.DataImport.Services;
using Absurdle.Engine;
using Absurdle.Engine.Services;
using Absurdle.UI;
using Absurdle.UI.Services;
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
        .AddTransient<CaseInsensitiveStringComparer>()
        .AddTransient<IGuessWordValidatorService, GuessWordValidatorService>(
            sp => new(
                new ReadWordCsvDataService(
                    validGuesses,
                    sp.GetRequiredService<CsvConfiguration>()
                ),
                sp.GetRequiredService<CaseInsensitiveStringComparer>(),
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
        .AddTransient<IAbsurdleEngine, AbsurdleEngine>()
        .AddTransient<IConsoleService, ConsoleService>()
        .AddHostedService<ConsoleApplication>()
    )
    .RunConsoleAsync();
