namespace Absurdle.Solver.Services
{
    public interface IConsole
    {
        public void WriteLine(string message);
        public string? ReadLine();
    }
}
