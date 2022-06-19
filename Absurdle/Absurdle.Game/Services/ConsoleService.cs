namespace Absurdle.Game.Services
{
    internal class ConsoleService : IConsoleService
    {
        public string? ReadLine() => Console.ReadLine();

        public void WriteLine(string message) => Console.WriteLine(message);
    }
}
