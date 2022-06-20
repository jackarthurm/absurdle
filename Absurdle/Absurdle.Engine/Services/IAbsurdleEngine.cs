using Absurdle.Engine.Model;

namespace Absurdle.Engine.Services
{
    public interface IAbsurdleEngine
    {
        public IEnumerable<CharacterHint>? BestWordHint { get; }

        public virtual bool IsSolved
            => BestWordHint?.All(
                result => result.Equals(
                    CharacterHint.PositionAndValueMatches
                )
            ) ?? false;

        public Task<bool> AddGuess(string guess, CancellationToken token = default);
    }
}
