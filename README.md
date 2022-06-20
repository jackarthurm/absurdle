Absurdle.DataImport - class library, contains services for reading word data from CSV files

Absurdle.Engine - class library, contains my absurdle "engine" and its supporting services. The engine is a state machine object. You instantiate it with a set of solution words and then keep adding guesses until it's in a solved state.

Absurdle.Game - console application, this is just a playable console UI for my absurdle engine

Absurdle.Solver - console application, uses the engine to answer some questions about how the engine behaves
