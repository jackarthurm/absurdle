namespace Absurdle.Engine.Services
{
    public interface IGuessWordValidatorService
    {
        public Task Init();
        public bool IsValid(string guessWord);
    }
}
