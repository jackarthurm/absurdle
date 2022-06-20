namespace Absurdle.Engine.Services
{
    public interface IGuessWordValidator
    {
        public Task Init(CancellationToken token = default);
        public bool IsValid(string guessWord);
    }
}
