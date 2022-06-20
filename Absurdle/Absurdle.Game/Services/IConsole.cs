namespace Absurdle.Game.Services
{
    public interface IConsole
    {
        public void WriteLine(string message);
        public string? ReadLine();
    }
}
