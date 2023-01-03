namespace ATMSimpleSimulation
{
    public class ATMSimulator
    {
        private IConsoleUserInput _console = new ConsoleReadLineRetriever();

        public void SimulateATM(ATM atm, Card card)
        {
            string? pin;
            string? option;

            while (true)
            {
                if (!atm.IsAutheticated())
                {
                    if (!atm.Read(card)) continue;
                    Console.Write("Digit your PIN: ");
                    pin = _console.GetUserInput();

                    if (pin == null) continue;
                    if (!atm.Authenticate(card, pin)) continue;
                }

                atm.ShowOptions();
                option = WaitForSelection(_console);
                atm.SelectOption(option, _console);
            }
        }

        static string WaitForSelection(IConsoleUserInput console)
        {
            string? userSelection = null;
            string[] availableOptions = new string[] { "1", "2", "3", "4", "5", "6" };
            while (string.IsNullOrEmpty(userSelection) || !availableOptions.Contains(userSelection))
            {
                Console.Write("Option: ");
                userSelection = console.GetUserInput();
            }
            return userSelection;
        }
    }
}
