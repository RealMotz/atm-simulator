namespace ATMSimpleSimulation
{
    public interface IConsoleUserInput
    {
        string? GetUserInput();
    }

    public class ConsoleReadLineRetriever : IConsoleUserInput
    {
        public string? GetUserInput() => Console.ReadLine();
    }
}
