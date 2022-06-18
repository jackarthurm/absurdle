namespace Absurdle.Engine.Services
{
    public interface IGuessWordValidatorService
    {
        public Task Init(CancellationToken token = default);
        public bool IsValid(string guessWord);
    }
}
