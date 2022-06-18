namespace Absurdle.Engine.Services
{
    public interface IAbsurdleEngine
    {
        public Task Init();

        public Task<bool> MakeGuess(string guess, CancellationToken token = default);

        public IEnumerable<CharacterHint> WordHint { get; }

        public virtual bool IsSolved
            => WordHint.All(
                result => result.Equals(
                    CharacterHint.PositionAndValueMatches
                )
            );
    }
}
