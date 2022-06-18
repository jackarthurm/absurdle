using Absurdle.Engine.Model;

namespace Absurdle.Engine.Services
{
    public interface IAbsurdleEngine
    {
        public Task Init(CancellationToken token = default);

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
