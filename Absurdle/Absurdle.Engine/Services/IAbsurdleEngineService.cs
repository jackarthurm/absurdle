﻿using Absurdle.Engine.Model;

namespace Absurdle.Engine.Services
{
    public interface IAbsurdleEngineService
    {
        public Task Init(CancellationToken token = default);

        public Task<bool> MakeGuess(string guess, CancellationToken token = default);

        public int PossibleSolutionsCount { get; }

        public IEnumerable<CharacterHint> WordHint { get; }

        public virtual bool IsSolved
            => WordHint.All(
                result => result.Equals(
                    CharacterHint.PositionAndValueMatches
                )
            );
    }
}
