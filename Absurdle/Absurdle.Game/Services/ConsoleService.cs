namespace Absurdle.Game.Services
{
    internal class ConsoleService : IConsole
    {
        public string? ReadLine() => Console.ReadLine();

        public void WriteLine(string message) => Console.WriteLine(message);
    }
}
