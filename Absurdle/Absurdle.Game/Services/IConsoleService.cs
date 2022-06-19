namespace Absurdle.Game.Services
{
    public interface IConsoleService
    {
        public void WriteLine(string message);
        public string? ReadLine();
    }
}
