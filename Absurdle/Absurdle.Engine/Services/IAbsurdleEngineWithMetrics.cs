namespace Absurdle.Engine.Services
{
    /// <summary>
    /// Exposes some useful data we can use to study the absurdle engine
    /// </summary>
    public interface IAbsurdleEngineWithMetrics : IAbsurdleEngine
    {
        /// <summary>
        /// This is the size of the "best" equivalence class
        /// </summary>
        public int PossibleSolutionsCount { get; }

        /// <summary>
        /// This is the number of equivalence classes
        /// </summary>
        public int WordHintsCount { get; }
    }
}
